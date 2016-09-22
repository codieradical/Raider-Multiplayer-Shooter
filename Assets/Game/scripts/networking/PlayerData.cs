using Raider.Game.GUI.Components;
using Raider.Game.Saves;
using UnityEngine.Networking;

namespace Raider.Game.Networking
{

    [System.Serializable]
    public class PlayerData : NetworkBehaviour
    {
        bool gotData = false;
        void Start()
        {
            this.transform.parent = NetworkManager.instance.lobbyGameObject.transform;

            if (isLocalPlayer)
            {
                UpdateLocalData(Session.saveDataHandler.GetUsername(), Session.activeCharacter);

                if (isServer)
                    RpcRecieveUpdateFromServer(this.username, serializedCharacter);
                else
                    CmdUpdateServer(this.username, serializedCharacter);
            }
        }

        public string username
        {
            get { return this.gameObject.name; }
            set { this.gameObject.name = value; }
        }

        public SaveDataStructure.Character character;

        #region serialization and syncing

        private string serializedCharacter
        {
            get { return Serialization.Serialize(character); }
            set { character = Serialization.Deserialize<SaveDataStructure.Character>(value); }
        }

        void UpdateLocalData(string _username, SaveDataStructure.Character _character)
        {
            this.username = _username;
            this.character = _character;
            gotData = true;
            NetworkManager.UpdateLobbyNameplates();
        }

        [Command]
        void CmdRequestUpdateFromServer()
        {
            if (gotData)
                TargetRecieveUpdateFromServer(connectionToClient, this.username, Serialization.Serialize(this.character));
        }

        [Command]
        void CmdUpdateServer(string _username, string _serializedCharacter)
        {
            RpcRecieveUpdateFromServer(_username, _serializedCharacter);
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter));
        }

        [ClientRpc]
        void RpcRecieveUpdateFromServer(string _username, string _serializedCharacter)
        {
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter));
        }

        [TargetRpc]
        void TargetRecieveUpdateFromServer(NetworkConnection conn, string _username, string _serializedCharacter)
        {
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter));
        }

        #endregion
    }
}