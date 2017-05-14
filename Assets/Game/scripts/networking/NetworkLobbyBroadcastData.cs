using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

namespace Raider.Game.Networking
{
    public class NetworkLobbyBroadcastData
    {
        public string JSONString
        {
            get
            {
                return JsonUtility.ToJson(this);
            }
        }

        public NetworkLobbyBroadcastData() { }

        public NetworkLobbyBroadcastData(string host, int port, string username, string gamemode, string map, bool teams)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.gamemode = gamemode;
            this.map = map;
            this.teams = teams;
        }

        public static NetworkLobbyBroadcastData FromJSONString(string json)
        {
            return JsonUtility.FromJson<NetworkLobbyBroadcastData>(json);
        }

        public static NetworkLobbyBroadcastData CurrentLobbyData()
        {
            try
            {
                return new NetworkLobbyBroadcastData(NetworkGameManager.instance.networkAddress, NetworkGameManager.instance.networkPort ,Session.userSaveDataHandler.GetUsername(), NetworkGameManager.instance.lobbySetup.syncData.gametype, NetworkGameManager.instance.lobbySetup.syncData.selectedScene, NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled);
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }

        public string GetTitle()
        {
            return host + ":" + port.ToString() + " - " + username;
        }

        public string GetDescription()
        {
            string description = gamemode + " on " + map;
            if (teams)
                description = "Team " + description;

            return description;
        }

        public string host;
        public int port;
        public string username;
        public string gamemode;
        public string map;
        public bool teams;
    }
}