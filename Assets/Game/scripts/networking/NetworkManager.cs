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

        public GameObject lobbyGameObject;

        List<PlayerData> players
        {
            get
            {
                List<PlayerData> _players = new List<PlayerData>();
                foreach(Transform playerObject in lobbyGameObject.transform)
                {
                    _players.Add(playerObject.GetComponent<PlayerData>());
                }
                return _players;
            }
        }


        // Use this for initialization
        void Start()
        {
            lobbyGameObject = new GameObject("_Lobby");
            DontDestroyOnLoad(lobbyGameObject);
        }
        
        new void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }

        public new void StartClient()
        {
            base.StartClient();
        }

        public new void StartHost()
        {
            base.StartHost();
        }

        public void UpdateUILobby()
        {
            LobbyHandler.DestroyAllPlayers();
            foreach(PlayerData newPlayer in players)
            {
                LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(newPlayer.username, false, false, false, newPlayer.character));
            }
        }

        #region Network Lobby Manager Overrides

        byte FindSlot()
        {
            for (byte i = 0; i < maxPlayers; i++)
            {
                if (lobbySlots[i] == null)
                {
                    return i;
                }
            }
            return byte.MaxValue;
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            string loadedSceneName = SceneManager.GetSceneAt(0).name;
            if (loadedSceneName != lobbyScene)
            {
                return;
            }

            // check MaxPlayersPerConnection
            int numPlayersForConnection = 0;
            foreach (var player in conn.playerControllers)
            {
                if (player.IsValid)
                    numPlayersForConnection += 1;
            }

            if (numPlayersForConnection >= maxPlayersPerConnection)
            {
                if (LogFilter.logWarn) { Debug.LogWarning("NetworkLobbyManager no more players for this connection."); }

                var errorMsg = new EmptyMessage();
                conn.Send(MsgType.LobbyAddPlayerFailed, errorMsg);
                return;
            }

            byte slot = FindSlot();
            if (slot == byte.MaxValue)
            {
                if (LogFilter.logWarn) { Debug.LogWarning("NetworkLobbyManager no space for more players"); }

                var errorMsg = new EmptyMessage();
                conn.Send(MsgType.LobbyAddPlayerFailed, errorMsg);
                return;
            }

            var newLobbyGameObject = OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
            if (newLobbyGameObject == null)
            {
                newLobbyGameObject = Instantiate(lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            }

            var newLobbyPlayer = newLobbyGameObject.GetComponent<NetworkLobbyPlayer>();
            newLobbyPlayer.slot = slot;
            lobbySlots[slot] = newLobbyPlayer;

            NetworkServer.AddPlayerForConnection(conn, newLobbyGameObject, playerControllerId);
        }

        #endregion
    }
}
