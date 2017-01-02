using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.GUI.Components;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System;
using Raider.Game.Scene;
using Raider.Game.Player;

namespace Raider.Game.Networking
{

    public class NetworkGameManager : NetworkLobbyManager
    {

        #region singleton setup

        //This class already inherits a singleton...
        public static NetworkGameManager instance;
        public GameObject lobbyGameObject;
        public LobbySetup lobbySetup;

        void Start()
        {
            GameObject lobby = new GameObject("_Lobby");
            lobbyGameObject = lobby;

            //Although this functionality is built into the network lobby manager,
            //It only works on update.
            DontDestroyOnLoad(this);
            lobbySetup = lobby.AddComponent<LobbySetup>();
            DontDestroyOnLoad(lobby);
            //For some reason it's not active...
            lobby.SetActive(true);
        }

        void Awake()
        {
            if (instance != null)
                Debug.LogError("More than one NetworkManager are active! What are you doing!!");
            instance = this;
        }

        void OnDestroy()
        {
#if !UNITY_EDITOR
            Debug.LogError("Something just destroyed the NetworkManager!");
#endif
            instance = null;
        }

        #endregion

        //This Queue is used to store function calls which will be processed next frame.
        //I really need to refactor this.
        public Queue<Action> actionQueue = new Queue<Action>();
        public delegate void NetworkMessage();

        void Update()
        {
            while(actionQueue.Count > 0)
            {
                actionQueue.Dequeue()();
            }
        }

        public List<NetworkLobbyPlayerSetup> LobbyPlayers
        {
            get
            {
                List<NetworkLobbyPlayerSetup> _players = new List<NetworkLobbyPlayerSetup>();
                foreach (Transform playerObject in lobbyGameObject.transform)
                {
                    _players.Add(playerObject.GetComponent<NetworkLobbyPlayerSetup>());
                }
                return _players;
            }
        }

        public PlayerData[] Players
        {
            get
            {
                if (Scenario.InLobby)
                {
                    List<PlayerData> players = new List<PlayerData>();
                    foreach (NetworkLobbyPlayerSetup lobbyPlayer in LobbyPlayers)
                    {
                        players.Add(lobbyPlayer.GetComponent<PlayerData>());
                    }
                    return players.ToArray();
                }
                else return FindObjectsOfType<PlayerData>();
            }
        }

        public bool IsLeader
        {
            get
            {
                if (PlayerData.localPlayerData != null)
                    if (PlayerData.localPlayerData.isLeader)
                        return true;
                    else
                        return false;
                else
                    return true;
            }
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            gamePlayer.GetComponent<PlayerData>().slot = lobbyPlayer.GetComponent<PlayerData>().slot;
            return true;
        }

        /// <summary>
        /// Get the slot number from the lobby or game player object.
        /// </summary>
        /// <returns>Returns the int slot.</returns>
        public int GetMyPlayerSlot()
        {
            return PlayerData.localPlayerData.slot;
        }

        public PlayerData GetPlayerDataBySlot(int gamePlayerSlot)
        {
            foreach(PlayerData player in Players)
            {
                if (player.GetComponent<PlayerData>().slot == gamePlayerSlot)
                    return player;
            }
            return null;
        }

        #region Lobby Methods

        //Called on a server or host when the server starts.
        public NetworkMessage onStartServer;
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (onStartServer != null)
                onStartServer();
        }

        //Called on a client when they connect.
        public NetworkMessage onClientConnect;
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            if (onClientConnect != null)
                onClientConnect();
        }

        //Called on a server or host when the server stops.
        public NetworkMessage onStopServer;
        public override void OnStopServer()
        {
            base.OnStopServer();
            if(onStopServer != null)
                onStopServer();
        }

        //Called on the client when they disconnect.
        public NetworkMessage onClientDisconnect;
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            if(onClientDisconnect != null)
                onClientDisconnect();
        }

        //Used to call SendReadyToBeginMessage on PlayerData from other classes.
        public void ReadyToBegin()
        {
            if (CurrentNetworkState == NetworkState.Client || CurrentNetworkState == NetworkState.Host)
                NetworkLobbyPlayerSetup.localPlayer.GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            foreach (PlayerController playerController in conn.playerControllers)
            {
                PlayerData playerData = playerController.gameObject.GetComponent<PlayerData>();

                if (playerData != null)
                {
                    NetworkLobbyPlayerSetup.localPlayer.GetComponent<PlayerChatManager>().CmdSendNotificationMessage("left the game.", playerData.slot);
                    break;
                }
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnLobbyServerPlayersReady()
        {
            NetworkLobbyPlayerSetup.localPlayer.RpcUpdateScenarioGametype();
            base.OnLobbyServerPlayersReady();
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            Scenario.instance.NetworkLoadedScene();
            base.OnLobbyClientSceneChanged(conn);
        }

        //TODO
        //IMPLEMENT COUNTDOWN TIMER
        //float countTimer = 0;

        //public override void OnLobbyServerPlayersReady()
        //{
        //    countTimer = Time.time + 5;
        //}

        //void Update()
        //{
        //    if (countTimer == 0)
        //        return;

        //    if (Time.time > countTimer)
        //    {
        //        countTimer = 0;
        //        ServerChangeScene(playScene);
        //    }
        //    else
        //    {
        //        Debug.Log("Counting down " + (countTimer - Time.time));
        //    }
        //}

        public void UpdateLobbyNameplates()
        {
            //If the player is in a lobby, use the player lobby objects to create nameplates.
            if (CurrentNetworkState != NetworkState.Offline)
            {
                LobbyHandler.DestroyAllPlayers();

                foreach (PlayerData playerData in Players)
                {
                    if (playerData.gotData)
                        LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(playerData.name, playerData.isLeader, false, false, playerData.character));
                    else
                        LobbyHandler.AddLoadingPlayer();
                }
            }
            //Otherwise, use their local data.
            else
            {
                LobbyHandler.DestroyAllPlayers();
                LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(Session.saveDataHandler.GetUsername(), true, false, false, Session.ActiveCharacter));
            }
        }

        #endregion

        #region Updating NetworkState

        public enum NetworkState
        {
            Offline,
            Client,
            Host,
            Server
            //Matchmaking?
        }

        //In order to make sure these delegates are called when required,
        //Lobby disconnect methods need to be overriden.
        public NetworkMessage onNetworkStateOffline;
        public NetworkMessage onNetworkStateHost;
        public NetworkMessage onNetworkStateClient;
        public NetworkMessage onNetworkStateServer;

        public NetworkState CurrentNetworkState
        {
            get
            {
                //Find what state the network is in, by checking if the server and or client are running.
                if (NetworkServer.active)
                    if (NetworkClient.active)
                        return NetworkState.Host;
                    else
                        return NetworkState.Server;
                else if (NetworkClient.active)
                    return NetworkState.Client;
                else
                    return NetworkState.Offline;
            }

            set
            {
                //Call methods to switch state by starting/stopping communications.
                if (value == NetworkState.Client)
                {
                    StartClient();
                    if(onNetworkStateClient != null)
                        onNetworkStateClient();
                }
                else if (value == NetworkState.Host)
                {
                    StartHost();
                    if (onNetworkStateHost != null)
                        onNetworkStateHost();
                }
                else if (value == NetworkState.Server)
                {
                    StartServer();
                    if (onNetworkStateHost != null)
                        onNetworkStateServer();
                }
                else if (value == NetworkState.Offline)
                {
                    StopCommunications();
                    if (onNetworkStateOffline != null)
                        onNetworkStateOffline();
                }

                //Sometimes this works now, sometimes it needs another frame.
                UpdateLobbyNameplates();
                actionQueue.Enqueue(UpdateLobbyNameplates);
            }
        }

        //This method doesn't call UpdateLobbyNameplates, so it shouldn't be called directly.
        private void StopCommunications()
        {
            //If the network is active, figure out what's running, and stop it.
            if (isNetworkActive)
            {
                if (CurrentNetworkState == NetworkState.Client)
                    StopClient();
                else if (CurrentNetworkState == NetworkState.Host)
                    StopHost();
                else if (CurrentNetworkState == NetworkState.Server)
                    StopServer();
            }
        }

        #endregion
    }
}
