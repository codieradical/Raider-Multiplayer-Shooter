using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.GUI.Components;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System;
using Raider.Game.Scene;

namespace Raider.Game.Networking
{

    public class NetworkManager : NetworkLobbyManager
    {

        #region singleton setup

        //This class already inherits a singleton...
        public static NetworkManager instance;
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

        void Update()
        {
            while(actionQueue.Count > 0)
            {
                actionQueue.Dequeue()();
            }
        }

        public List<LobbyPlayerData> Players
        {
            get
            {
                List<LobbyPlayerData> _players = new List<LobbyPlayerData>();
                foreach (Transform playerObject in lobbyGameObject.transform)
                {
                    _players.Add(playerObject.GetComponent<LobbyPlayerData>());
                }
                return _players;
            }
        }

        public bool IsLeader
        {
            get
            {
                if (LobbyPlayerData.localPlayer != null)
                    if (LobbyPlayerData.localPlayer.isLeader)
                        return true;
                    else
                        return false;
                else
                    return true;
            }
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            gamePlayer.GetComponent<Player.Player>().slot = lobbyPlayer.GetComponent<NetworkLobbyPlayer>().slot;
            return true;
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            //Make sure the lobby player is still active.
            LobbyPlayerData.localPlayer.gameObject.SetActive(true);
        }

        public LobbyPlayerData GetLobbyPlayerBySlot(int gamePlayerSlot)
        {
            foreach(LobbyPlayerData player in Players)
            {
                if (player.GetComponent<NetworkLobbyPlayer>().slot == gamePlayerSlot)
                    return player;
            }
            return null;
        }

        #region Lobby Methods

        //Used to call SendReadyToBeginMessage on PlayerData from other classes.
        public void ReadyToBegin()
        {
            if (CurrentNetworkState == NetworkState.Client || CurrentNetworkState == NetworkState.Host)
                LobbyPlayerData.localPlayer.GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
        }

        public override void OnLobbyServerPlayersReady()
        {
            LobbyPlayerData.localPlayer.RpcUpdateScenarioGametype();
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

                foreach (LobbyPlayerData playerData in Players)
                {
                    if (playerData.gotData)
                        LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(playerData.Username, playerData.isLeader, false, false, playerData.character));
                    else
                        LobbyHandler.AddLoadingPlayer();
                }
            }
            //Otherwise, use their local data.
            else
            {
                LobbyHandler.DestroyAllPlayers();
                LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(Session.saveDataHandler.GetUsername(), true, false, false, Session.activeCharacter));
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
                    StartClient();
                else if (value == NetworkState.Host)
                    StartHost();
                else if (value == NetworkState.Server)
                    StartServer();
                else if (value == NetworkState.Offline)
                    StopCommunications();

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
