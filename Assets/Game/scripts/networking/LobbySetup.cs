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
        { get { return gametype; }
            set
            {
                gametype = value;
                LobbySetupPane.instance.UpdatePaneData();

                if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Server)
                    RpcSendLobbySetup(gametype, network, selectedScene);
            }
        }
        private string selectedScene;
        public string SelectedScene
        { get { return selectedScene; }
            set
            {
                selectedScene = value;
                LobbySetupPane.instance.UpdatePaneData();

                if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Server)
                    RpcSendLobbySetup(gametype, network, selectedScene);
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
                    RpcSendLobbySetup(gametype, network, selectedScene);
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

        //If a new player joins the lobby, this is used to send them the details.
        [TargetRpc]
        public void TargetSendLobbySetup(NetworkConnection conn, string _gametype, string _network, string _selectedScene)
        {
            Gametype = _gametype;
            Network = _network;
            SelectedScene = _selectedScene;
        }

        //If the host changes the lobby setup, this sends the new details to the clients.
        [ClientRpc]
        public void RpcSendLobbySetup(string _gametype, string _network, string _selectedScene)
        {
            Gametype = _gametype;
            Network = _network;
            SelectedScene = _selectedScene;
        }
    }
}