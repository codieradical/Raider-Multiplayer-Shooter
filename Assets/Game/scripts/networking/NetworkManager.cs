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

        //These methods aren't working.
        //But they should be here anyway.
        //I need the start client/server buttons to check if the user is logged in.

        //public new void StartClient()
        //{
        //    if (Session.activeCharacter != null)
        //        base.StartClient();
        //    else
        //        Debug.Log("Can't join server when you haven't selected a character!");
        //}

        //public new void StartHost()
        //{
        //    if (Session.activeCharacter != null)
        //        base.StartHost();
        //    else
        //        Debug.Log("Can't host server when you haven't selected a character!");
        //}

        #region Lobby Methods

        public void UpdateLobbyNameplates()
        {
            LobbyHandler.DestroyAllPlayers();

            foreach(PlayerData playerData in players)
            {
                if (playerData.gotData)
                    LobbyHandler.AddPlayer(new LobbyHandler.PlayerNameplate(playerData.username, playerData.isHost, false, false, playerData.character));
                else
                    LobbyHandler.AddLoadingPlayer();
            }
        }

        #endregion
    }
}
