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

        void Start()
        {
            GameObject lobby = new GameObject("_Lobby");
            lobbyGameObject = lobby;

            //Although this functionality is built into the network lobby manager,
            //It only works on update.
            DontDestroyOnLoad(this);
            lobby.AddComponent<NetworkLobbySettings>();
            DontDestroyOnLoad(lobby);
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

        public List<PlayerData> players
        {
            get
            {
                List<PlayerData> _players = new List<PlayerData>();
                foreach (Transform playerObject in lobbyGameObject.transform)
                {
                    _players.Add(playerObject.GetComponent<PlayerData>());
                }
                return _players;
            }
        }

        public PlayerData GetMyLobbyPlayer()
        {
            foreach(PlayerData player in players)
            {
                if (player.isLocalPlayer)
                    return player;
            }
            return null;
        }

        #region Lobby Methods

        public void UpdateLobbyNameplates()
        {
            LobbyHandler.DestroyAllPlayers();

            foreach(PlayerData playerData in players)
            {
                if (playerData.gotData)
                    LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(playerData.username, playerData.isLeader, false, false, playerData.character));
                else
                    LobbyHandler.AddLoadingPlayer();
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
            }
        }

        public void StopCommunications()
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
