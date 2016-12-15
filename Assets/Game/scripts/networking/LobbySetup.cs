using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Raider.Game.GUI.Components;
using Raider.Game.Scene;
using System;

namespace Raider.Game.Networking
{
    public class LobbySetup : NetworkBehaviour
    {
        private string gametype;
        public string GametypeString
        {
            get { return gametype; }
            set
            {
                gametype = value;
                LobbySetupPane.instance.UpdatePaneData();

                if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server)
                    LobbyPlayerData.localPlayer.RpcSendLobbySetup(gametype, network, selectedScene);
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

                if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server)
                    LobbyPlayerData.localPlayer.RpcSendLobbySetup(gametype, network, selectedScene);
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

                if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server)
                    LobbyPlayerData.localPlayer.RpcSendLobbySetup(gametype, network, selectedScene);
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