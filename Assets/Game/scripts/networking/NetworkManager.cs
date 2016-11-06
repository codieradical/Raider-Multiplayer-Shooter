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

        //public List<PlayerData> players
        //{
        //    get
        //    {
        //        List<PlayerData> _players = new List<PlayerData>();
        //        foreach(Transform playerObject in lobbyGameObject.transform)
        //        {
        //            _players.Add(playerObject.GetComponent<PlayerData>());
        //        }
        //        return _players;
        //    }
        //}

        public new void StartClient()
        {
            if (Session.activeCharacter != null)
                base.StartClient();
            else
                Debug.Log("Can't join server when you haven't selected a character!");
        }

        public new void StartHost()
        {
            if (Session.activeCharacter != null)
                base.StartHost();
            else
                Debug.Log("Can't host server when you haven't selected a character!");
        }

        #region Lobby Methods

        //public override void OnServerDisconnect(NetworkConnection conn)
        //{
        //    PlayerData removePlayer = null;
        //    foreach (PlayerData player in players)
        //    {
        //        if (player.connectionToClient == conn)
        //            removePlayer = player;
        //    }
        //    if(removePlayer != null)
        //    {
        //        removePlayer.CmdDestroyPlayerData();
        //        PlayerData.UpdateClientPlayerDataObjects(); // RPC to update nameplates.
        //        UpdateLobbyNameplates(); //Update the server too.
        //    }
        //    else
        //        Debug.LogError("Failed to remove a lobby object");

        //    base.OnServerDisconnect(conn);
        //}

        public void UpdateLobbyNameplates()
        {
            LobbyHandler.DestroyAllPlayers();
            foreach (NetworkLobbyPlayer newPlayer in lobbySlots)
            {
                PlayerData playerData = newPlayer.gameObject.GetComponent<PlayerData>();
                if (playerData.gotData)
                    LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(playerData.username, playerData.isHost, false, false, playerData.character));
                else
                    LobbyHandler.AddLoadingPlayer();
            }
        }

        public override void OnLobbyServerConnect(NetworkConnection conn)
        {
            UpdateLobbyNameplates();
        }
        public override void OnLobbyClientConnect(NetworkConnection conn)
        {
            UpdateLobbyNameplates();
        }

        #endregion


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
