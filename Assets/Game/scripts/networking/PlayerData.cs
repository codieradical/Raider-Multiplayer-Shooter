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
            //Looks like I'd have to rewrite the entire network manager just to allow this so... guess not.
            //this.transform.parent = NetworkManager.instance.lobbyGameObject.transform;

            //if (isLocalPlayer)
            //{
            //    UpdateLocalData(Session.saveDataHandler.GetUsername(), Session.activeCharacter, Network.isServer);

            //    if (isHost)
            //        RpcRecieveUpdateFromServer(this.username, serializedCharacter, isHost);
            //    else
            //        CmdUpdateServer(this.username, serializedCharacter, isHost);
            //}
        }

        //Player DATA
        public string username
        {
            get { return this.gameObject.name; }
            set { this.gameObject.name = value; }
        }
        public SaveDataStructure.Character character;
        public bool isHost;

        #region serialization and syncing

        //public string serializedCharacter
        //{
        //    get { return Serialization.Serialize(character); }
        //    set { character = Serialization.Deserialize<SaveDataStructure.Character>(value); }
        //}

        //void UpdateLocalData(string _username, SaveDataStructure.Character _character, bool _isHost)
        //{
        //    this.username = _username;
        //    this.character = _character;
        //    this.isHost = _isHost;
        //    this.gotData = true;
        //    NetworkManager.UpdateLobbyNameplates();
        //}

        //[Command]
        //void CmdUpdateServer(string _username, string _serializedCharacter, bool _isHost)
        //{
        //    RpcRecieveUpdateFromServer(_username, _serializedCharacter, _isHost);
        //    UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter), _isHost);

        //    //If the client has sent their player data to the server, 
        //    //that means it's spawned and ready to recieve data regarding other players.
        //    //So now the server sends that data using the Client RPC.
        //    UpdateClientPlayerDataObjects();
        //}

        //[Command]
        //public void CmdDestroyPlayerData()
        //{
        //    NetworkServer.Destroy(this.gameObject);
        //}

        //[ClientRpc]
        //public void RpcRecieveUpdateFromServer(string _username, string _serializedCharacter, bool _isHost)
        //{
        //    UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter), _isHost);
        //}

        //[Server]
        //public static void UpdateClientPlayerDataObjects()
        //{
        //    foreach (PlayerData player in NetworkManager.instance.players)
        //    {
        //        if (player.gotData)
        //            player.RpcRecieveUpdateFromServer(player.username, player.serializedCharacter, player.isHost);
        //    }
        //}

        //[TargetRpc]
        //public void TargetUpdateLobbyNameplates(NetworkConnection conn)
        //{
        //    NetworkManager.UpdateLobbyNameplates();
        //}

        #endregion
    }
}