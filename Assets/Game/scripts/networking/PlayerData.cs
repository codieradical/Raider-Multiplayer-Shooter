using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.Saves;
using Raider.Game.Scene;
using UnityEngine.Networking;
using Raider.Game.Networking;
using Raider;

[System.Serializable]
public class PlayerData : NetworkBehaviour {

    void Start()
    {
        this.transform.parent = Raider.Game.Networking.NetworkManager.instance.lobbyGameObject.transform;

        if (isLocalPlayer)
        {
            UpdateLocalData(Session.saveDataHandler.GetUsername(), Session.activeCharacter);

            Raider.Game.Networking.NetworkManager.instance.UpdateUILobby();

            if (!isServer)
                CmdUpdateServer(this.username, this.character);
        }
        else
        {
            CmdRequestUpdateFromServer();
        }
    }

    void UpdateLocalData(string _username, SaveDataStructure.Character _character)
    {
        this.username = _username;
        this.character = _character;
        this.transform.gameObject.name = this.username;
        Raider.Game.Networking.NetworkManager.instance.UpdateUILobby();
    }

    [Command]
    void CmdUpdateServer(string _username, SaveDataStructure.Character _character)
    {
        UpdateLocalData(_username, _character);
    }

    [Command]
    void CmdRequestUpdateFromServer()
    {
        TargetRecieveUpdateFromServer(connectionToClient, this.username, this.character);
    }

    [TargetRpc]
    void TargetRecieveUpdateFromServer(NetworkConnection target, string _username, SaveDataStructure.Character _character)
    {
        UpdateLocalData(_username, _character);
    }

    [SyncVar]
    public string username;
    [SyncVar]
    public SaveDataStructure.Character character;
}
