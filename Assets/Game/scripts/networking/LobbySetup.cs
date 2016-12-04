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
        public string Gametype
        {
            get { return gametype; }
            set
            {
                gametype = value;
                LobbySetupPane.instance.UpdatePaneData();

                if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Server)
                    NetworkManager.instance.GetMyLobbyPlayer().RpcSendLobbySetup(gametype, network, selectedScene);
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

                if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Server)
                    NetworkManager.instance.GetMyLobbyPlayer().RpcSendLobbySetup(gametype, network, selectedScene);
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

                if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Server)
                    NetworkManager.instance.GetMyLobbyPlayer().RpcSendLobbySetup(gametype, network, selectedScene);
            }
        }


        public Scenario.Gametype scenarioGametype
        {
            get
            {
                //Replace spaces in the string with underscores, parse.
                return (Scenario.Gametype)(Enum.Parse(typeof(Scenario.Gametype), Gametype.Replace(" ", "_")));
            }
            set
            {
                Gametype = value.ToString().Replace("_", " ");
            }
        }
    }
}