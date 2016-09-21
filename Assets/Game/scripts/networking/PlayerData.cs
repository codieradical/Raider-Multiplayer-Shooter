using Raider.Game.GUI.Components;
using Raider.Game.Saves;
using UnityEngine.Networking;

namespace Raider.Game.Networking
{

    [System.Serializable]
    public class PlayerData : NetworkBehaviour
    {

        void Start()
        {
            this.transform.parent = NetworkManager.instance.lobbyGameObject.transform;

            if (isLocalPlayer)
            {
                UpdateLocalData(Session.saveDataHandler.GetUsername(), Session.activeCharacter);

                if (!isServer)
                    CmdUpdateServer(this.username, Serialization.Serialize(character));
            }
            else
            {
                CmdRequestUpdateFromServer();
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
            NetworkManager.UpdateLobbyNameplates();
        }

        [Command]
        void CmdUpdateServer(string _username, string _serializedCharacter)
        {
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter));
        }

        [Command]
        void CmdRequestUpdateFromServer()
        {
            RpcRecieveUpdateFromServer(this.username, Serialization.Serialize(this.character));
        }

        [ClientRpc]
        void RpcRecieveUpdateFromServer(string _username, string _serializedCharacter)
        {
            UpdateLocalData(_username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter));
        }

        #endregion
    }
}