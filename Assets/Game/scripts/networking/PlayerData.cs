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
            character = Session.activeCharacter;
            currentScenario = Scenario.instance;
            username = Session.saveDataHandler.GetUsername();
            this.gameObject.name = username;
        }
    }

    public string username;
    public SaveDataStructure.Character character;
    public Scenario currentScenario;

}
