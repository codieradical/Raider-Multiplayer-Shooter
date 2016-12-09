using Raider.Game.GUI.Components;
using Raider.Game.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
    /// <summary>
    /// This class used to be dedicated to the join lobby screen.
    /// Now it's more of a network controller. So it could use some refactoring.
    /// </summary>
    public class MatchmakingHandler : MonoBehaviour {

        public void BackOut()
        {
            MenuManager.instance.ShowMenu(MainmenuHandler.instance.MainMenuScreen.GetComponent<Menu>());
            GametypeButtons.instance.ShowButtons();
        }

        public void JoinGame(Text ipTxt)
        {
            if(Session.activeCharacter == null)
            {
                Debug.LogWarning("User attempted to join a server with no character selected!");
            }
            if(NetworkManager.instance.CurrentNetworkState != NetworkManager.NetworkState.Offline)
            {
                Debug.LogWarning("User attempted to join a server while already in a server!");
            }
            NetworkManager.instance.networkAddress = ipTxt.text;
            NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Client;
            //If the player sucessfully joined a game...
            if(NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Client)
            {
                //Grab the lobby details.
                GametypeButtons.instance.HideButtons();
                MenuManager.instance.ShowMenu(MainmenuHandler.instance.MainMenuScreen.GetComponent<Menu>());
                LobbySetupPane.instance.OpenPane();
            }
        }

        public void OpenNetworkOptions()
        {
            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            options.Add(new OptionsPaneOption.OptionsPaneContents("Offline", "Splitscreen co-op. Not Yet Implemented."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Local", "Host a Local Area Nework Lobby. Not Yet Implemented."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Online", "Host an online lobby on your PC"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Online Server", "Host an online server lobby on your PC"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Matchmaker/Dedicated", "Not yet implemented."));

            OptionsPaneHandler.instance.ShowOptions("Network", options, SelectNetwork);
        }

        public void SelectNetwork(string option)
        {
            NetworkManager.instance.lobbySetup.Network = option;
            //The user might be switching from host to server, so it's important to end communications first.
            if (option == "Online")
            {
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Host;
            }

            if (option == "Online Server")
            {
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Server;
            }

        }
    }
}