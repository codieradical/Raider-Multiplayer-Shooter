using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Raider.Game.GUI.Components;
using Raider.Game.Scene;
using Raider.Game.Player;
using System;

namespace Raider.Game.Networking
{
    public class LobbySetup : MonoBehaviour
    {
        private string gametype;
        public string GametypeString
        {
            get { return gametype; }
            set
            {
                gametype = value;
                LobbySetupPane.instance.UpdatePaneData();

                SendLobbySetupUpdate();
            }
        }
        private string selectedScene;
        public string SelectedScene
        {
            get { return selectedScene; }
            set
            {
                selectedScene = value;
                LobbySetupPane.instance.UpdatePaneData();

                SendLobbySetupUpdate();
            }
        }
        private string network;
        public string Network
        {
            get { return network; }
            set
            {
                network = value;
                LobbySetupPane.instance.UpdatePaneData();

                SendLobbySetupUpdate();
            }
        }


        public Scenario.Gametype Gametype
        {
            get
            {
                //Replace spaces in the string with underscores, parse.
                return (Scenario.Gametype)(Enum.Parse(typeof(Scenario.Gametype), GametypeString.Replace(" ", "_")));
            }
            set
            {
                GametypeString = value.ToString().Replace("_", " ");
            }
        }

        //Used to send lobby setup changes over a network.
        public void SendLobbySetupUpdate()
        {
            //If the player is offline, there's nowhere to send the data...
            if(NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline && NetworkLobbyPlayerSetup.localPlayer.playerData.syncData.isLeader)
                NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(gametype, network, selectedScene);
        }

        //Used to recieve lobby setup changes without causing a syncing loop by changing private property backends.
        public void RecieveLobbySetupUpdate(string _gametype, string _selectedScene, string _network)
        {
            gametype = _gametype;
            selectedScene = _selectedScene;
            network = _network;

            if(LobbySetupPane.instance != null)
                LobbySetupPane.instance.UpdatePaneData();
        }

        public enum Teams
        {
            None = -1,
            Red = 0,
            Blue = 1,
            Green = 2,
            Yellow = 3,
            Pink = 4,
            Brown = 5
        }
    }
}