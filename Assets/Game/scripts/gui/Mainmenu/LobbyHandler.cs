using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class LobbyHandler : MonoBehaviour {

    private int maxPlayers = 6;

    public UnityEngine.Object nameplatePrefab;

    private List<PlayerNameplate> players = new List<PlayerNameplate>();

    public struct PlayerNameplate
    {
        public PlayerNameplate(string _username, bool _leader, bool _speaking, bool _canspeak, SaveDataStructure.Character _character)
        {
            username = _username;
            leader = _leader;
            speaking = _speaking;
            canspeak = _canspeak;
            character = _character;
        }

        public string username;
        public bool leader;
        public bool speaking;
        public bool canspeak;

        public SaveDataStructure.Character character;
    }

    public void AddPlayer(PlayerNameplate player)
    {
        players.Add(player);

        GameObject newPlayer = Instantiate(nameplatePrefab) as GameObject;

        newPlayer.GetComponent<PreferredSizeOverride>().providedGameObject = gameObject;

        newPlayer.name = player.username;
        newPlayer.transform.FindChild("emblem").GetComponent<EmblemHandler>().UpdateEmblem(player.character);

        newPlayer.transform.SetParent(gameObject.transform.FindChild("Players"), false);
        newPlayer.transform.FindChild("name").GetComponent<Text>().text = player.username;
        newPlayer.transform.FindChild("guild").GetComponent<Text>().text = player.character.guild;
        newPlayer.transform.FindChild("level").GetComponent<Text>().text = player.character.level.ToString();
        newPlayer.transform.FindChild("icons").FindChild("leader").gameObject.SetActive(player.leader);

        //Color plateColor = player.character.armourPrimaryColor.color;
        //plateColor.a = 0.5f;
        //newPlayer.GetComponent<Image>().color = plateColor;

        UpdateSidebar();
    }

    void UpdateSidebar()
    {
        transform.FindChild("Sidebar").FindChild("Player Count").GetComponent<Text>().text = String.Format("{0}/{1}", players.Count.ToString(), maxPlayers);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
