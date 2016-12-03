using Raider.Game.Saves;
using UnityEngine.Networking;
using UnityEngine;

namespace Raider.Game.Networking
{
    [System.Serializable]
    public class PlayerData : NetworkBehaviour
    {
        /// <summary>
        /// Is the PlayerData up to date?
        /// </summary>
        public bool gotData = false;
        void Start()
        {
            this.transform.SetParent(NetworkManager.instance.lobbyGameObject.transform);

            if (isLocalPlayer)
            {
                //If the player is hosting (if networkserver is active), isLeader will be true.
                UpdateLocalData(Session.saveDataHandler.GetUsername(), Session.activeCharacter, NetworkServer.active);

                if (isLeader)
                    RpcRecieveUpdateFromServer(this.username, serializedCharacter, isLeader);
                else
                    CmdUpdateServer(this.username, serializedCharacter, isLeader);

                //If the player is not the host, they're automatically set to ready.
                //This means the host's ready flag starts the game.
                if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Client)
                    GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
            }
        }

        //Player DATA
        public string username
        {
            get { return this.gameObject.name; }
            set { this.gameObject.name = value; }
        }
        public SaveDataStructure.Character character;
        public bool isLeader;

        #region serialization and syncing

        public string serializedCharacter
        {
            get { return Serialization.Serialize(character); }
            set { character = Serialization.Deserialize<SaveDataStructure.Character>(value); }
        }

        void UpdateLocalData(string _username, SaveDataStructure.Character _character, bool _isLeader)
        {
            this.username = _username;
            this.character = _character;
            this.isLeader = _isLeader;
            this.gotData = true;
            NetworkManager.instance.UpdateLobbyNameplates();
        }

        [Command]
        void CmdUpdateServer(string _username, string _serializedCharacter, bool _isHost)
        {
            RpcRecieveUpdateFromServer(_username, _serializedCharacter, _isHost);
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter), _isHost);

            //If the client has sent their player data to the server, 
            //that means it's spawned and ready to recieve data regarding other players.
            //So now the server sends that data using the Client RPC.
            UpdateClientPlayerDataObjects();
        }

        [ClientRpc]
        public void RpcRecieveUpdateFromServer(string _username, string _serializedCharacter, bool _isHost)
        {
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter), _isHost);
        }

        [Server]
        public static void UpdateClientPlayerDataObjects()
        {
            foreach (PlayerData player in NetworkManager.instance.players)
            {
                if (player.gotData)
                    player.RpcRecieveUpdateFromServer(player.username, player.serializedCharacter, player.isLeader);
            }
        }

        #endregion

        //If a player is remove from the scene, update the lobby!
        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            NetworkManager.instance.actionQueue.Enqueue(NetworkManager.instance.UpdateLobbyNameplates);
        }
    }
}