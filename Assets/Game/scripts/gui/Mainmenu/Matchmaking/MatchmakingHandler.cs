using Raider.Game.GUI.Components;
using Raider.Game.Networking;
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
            MenuManager.instance.ShowMenu(MainmenuController.instance.MainMenuScreen.GetComponent<Menu>());
            GametypeButtons.instance.ShowButtons();
        }

        public void JoinGame(Text ipTxt)
        {
            if(Session.ActiveCharacter == null)
            {
                Debug.LogWarning("User attempted to join a server with no character selected!");
            }
            if(NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
            {
                Debug.LogWarning("User attempted to join a server while already in a server!");
            }
            NetworkGameManager.instance.networkAddress = ipTxt.text;
            NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Client;
            //If the player sucessfully joined a game...
            if(NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
            {
                //Grab the lobby details.
                GametypeButtons.instance.HideButtons();
                MenuManager.instance.ShowMenu(MainmenuController.instance.MainMenuScreen.GetComponent<Menu>());
                LobbySetupPane.instance.OpenPane();
            }
        }
    }
}