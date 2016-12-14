using Raider.Game.GUI.Screens;
using Raider.Game.Networking;
using Raider.Game.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Raider.Game.GUI.StartMenu
{
    public class StartMenuGame : StartMenuPane
    {

        public Button leaveGameButton;
        public Button endGameButton;
        public Button changeCharacterButton;
        public Button logOutButton;

        protected override void SetupPaneData()
        {
            leaveGameButton.gameObject.SetActive(false);
            endGameButton.gameObject.SetActive(false);
            changeCharacterButton.gameObject.SetActive(false);
            logOutButton.gameObject.SetActive(false);

            leaveGameButton.onClick.RemoveAllListeners();
            endGameButton.onClick.RemoveAllListeners();
            changeCharacterButton.onClick.RemoveAllListeners();
            logOutButton.onClick.RemoveAllListeners();

            if (Scenario.InLobby)
            {
                changeCharacterButton.gameObject.SetActive(true);
                logOutButton.gameObject.SetActive(true);
                changeCharacterButton.onClick.AddListener(() => StartMenuHandler.instance.CloseStartMenu());
                changeCharacterButton.onClick.AddListener(() => MainmenuHandler.instance.ChangeCharacter());
                logOutButton.onClick.AddListener(() => StartMenuHandler.instance.CloseStartMenu());
                logOutButton.onClick.AddListener(() => MainmenuHandler.instance.Logout());
            }
            else
            {
                if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Client)
                {
                    leaveGameButton.onClick.AddListener(() => StartMenuHandler.instance.CloseStartMenu());
                    leaveGameButton.onClick.AddListener(() => Scenario.instance.LeaveGame());
                    leaveGameButton.gameObject.SetActive(true);
                }
                else
                {
                    endGameButton.onClick.AddListener(() => StartMenuHandler.instance.CloseStartMenu());
                    endGameButton.onClick.AddListener(() => Scenario.instance.LeaveGame());
                    endGameButton.gameObject.SetActive(true);
                }
            }

        }
    }
}
