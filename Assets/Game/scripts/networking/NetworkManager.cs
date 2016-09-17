using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.Networking
{

    public class NetworkManager : UnityEngine.Networking.NetworkLobbyManager
    {
        #region singleton setup

        //This class already inherits a singleton...
        NetworkManager instance;

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


        GameObject lobby;
        List<PlayerData> players
        {
            get
            {
                List<PlayerData> _players = new List<PlayerData>();
                foreach(Transform playerObject in lobby.transform)
                {
                    _players.Add(playerObject.GetComponent<PlayerData>());
                }
                return _players;
            }
        }


        // Use this for initialization
        void Start()
        {
            lobby = new GameObject("_Lobby");
            DontDestroyOnLoad(lobby);
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
    }
}
