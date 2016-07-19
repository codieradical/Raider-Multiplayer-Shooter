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
        public PlayerNameplate(string _username, int _emblemLayer1, int _emblemLayer2, int _emblemLayer3, string _guild, int _level, bool _leader, bool _speaking, bool _canspeak)
        {
            username = _username;
            emblemLayer1 = _emblemLayer1;
            emblemLayer2 = _emblemLayer2;
            emblemLayer3 = _emblemLayer3;
            guild = _guild;
            level = _level;
            leader = _leader;
            speaking = _speaking;
            canspeak = _canspeak;
        }

        public string username;
        public int emblemLayer1;
        public int emblemLayer2;
        public int emblemLayer3;
        public string guild;
        public int level;
        public bool leader;
        public bool speaking;
        public bool canspeak;
    }

    public void AddPlayer(PlayerNameplate player)
    {
        players.Add(player);

        GameObject newPlayer = Instantiate(nameplatePrefab) as GameObject;

        newPlayer.GetComponent<PreferredSizeOverride>().providedGameObject = gameObject;

        newPlayer.transform.SetParent(gameObject.transform.FindChild("Players"), false);
        newPlayer.transform.FindChild("name").GetComponent<Text>().text = player.username;
        newPlayer.transform.FindChild("guild").GetComponent<Text>().text = player.guild;
        newPlayer.transform.FindChild("level").GetComponent<Text>().text = player.level.ToString();
        newPlayer.transform.FindChild("icons").FindChild("leader").gameObject.SetActive(player.leader);

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
