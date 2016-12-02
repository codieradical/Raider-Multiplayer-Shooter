using Raider.Game.GUI.Components;
using Raider.Game.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
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
            if(NetworkManager.instance.isNetworkActive)
            {
                Debug.LogWarning("User attempted to join a server while already in a server!");
            }
            NetworkManager.instance.networkAddress = ipTxt.text;
            NetworkManager.instance.StartClient();
        }
    }
}