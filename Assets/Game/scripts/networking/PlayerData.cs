using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.Saves;
using Raider.Game.Scene;
using UnityEngine.Networking;
using Raider;

public class PlayerData : NetworkBehaviour {

    void Start()
    {
        if (isLocalPlayer)
        {
            this.character = Session.activeCharacter;
            //currentScenario = Scenario.instance;
            this.username = Session.saveDataHandler.GetUsername();
            CmdSendPlayerData(username, character);
            this.gameObject.name = username;
            Raider.Game.Networking.NetworkManager.instance.UpdateUILobby();
        }
    }

    [Command]
    void CmdSendPlayerData(string _username, SaveDataStructure.Character _character)
    {
        this.username = _username;
        this.character = _character;
        this.gameObject.name = username;
        Raider.Game.Networking.NetworkManager.instance.UpdateUILobby();
    }

    [SyncVar]
    public string username;
    [SyncVar]
    public SaveDataStructure.Character character;
    //[SyncVar]
    //public Scenario currentScenario;

}
