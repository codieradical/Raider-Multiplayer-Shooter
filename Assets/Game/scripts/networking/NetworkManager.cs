using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.GUI.Components;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System;

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
            Debug.LogError("Something just destroyed the NetworkManager!");
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

        public List<LobbyPlayerData> players
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

        public LobbyPlayerData GetMyLobbyPlayer()
        {
            foreach(LobbyPlayerData player in players)
            {
                if (player.isLocalPlayer)
                    return player;
            }
            return null;
        }

        public bool isLeader
        {
            get
            {
                if (GetMyLobbyPlayer() != null)
                    if (GetMyLobbyPlayer().isLeader)
                        return true;
                    else
                        return false;
                else
                    return true;
            }
        }

        //Used to call SendReadyToBeginMessage on PlayerData from other classes.
        public void ReadyToBegin()
        {
            if (currentNetworkState == NetworkState.Client || currentNetworkState == NetworkState.Host)
                GetMyLobbyPlayer().GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
        }

        #region Lobby Methods

        public void UpdateLobbyNameplates()
        {
            //If the player is in a lobby, use the player lobby objects to create nameplates.
            if (currentNetworkState != NetworkState.Offline)
            {
                LobbyHandler.DestroyAllPlayers();

                foreach (LobbyPlayerData playerData in players)
                {
                    if (playerData.gotData)
                        LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(playerData.username, playerData.isLeader, false, false, playerData.character));
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

        public NetworkState currentNetworkState
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
                if (currentNetworkState == NetworkState.Client)
                    StopClient();
                else if (currentNetworkState == NetworkState.Host)
                    StopHost();
                else if (currentNetworkState == NetworkState.Server)
                    StopServer();
            }
        }

        #endregion
    }
}
