using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.Screens;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.StartMenu
{
    public class StartMenuGame : StartMenuPane
    {
        [Header("Option Buttons")]
        public Button leaveGameButton;
        public Button endGameButton;
        public Button changeCharacterButton;
        public Button logOutButton;
        public Button debugButton;
        public Button changeTeamButton;

        public Image optionImage;
        public Text optionText;

        public Sprite leaveGameSprite;
        public Sprite endGameSprite;
        public Sprite changeCharacterSprite;
        public Sprite logOutSprite;
        public Sprite changeTeamSprite;

        protected override void SetupPaneData()
        {
            NoHover();

            leaveGameButton.gameObject.SetActive(false);
            endGameButton.gameObject.SetActive(false);
            changeCharacterButton.gameObject.SetActive(false);
            logOutButton.gameObject.SetActive(false);
            changeTeamButton.gameObject.SetActive(false);
            

            leaveGameButton.onClick.RemoveAllListeners();
            endGameButton.onClick.RemoveAllListeners();
            changeCharacterButton.onClick.RemoveAllListeners();
            logOutButton.onClick.RemoveAllListeners();
            changeTeamButton.onClick.RemoveAllListeners();

            if(NetworkGameManager.instance.lobbySetup.syncData.gameOptions != null && NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
            {
                if(Scenario.InLobby && NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.clientTeamChangingLobby)
                {
                    changeTeamButton.gameObject.SetActive(true);
                }
                else if(NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.clientTeamChangingGame)
                {
                    changeTeamButton.gameObject.SetActive(true);
                }

            }

            if (Scenario.InLobby)
            {
                changeCharacterButton.gameObject.SetActive(true);
                logOutButton.gameObject.SetActive(true);
                changeCharacterButton.onClick.AddListener(() => StartMenuHandler.instance.CloseStartMenu());
                changeCharacterButton.onClick.AddListener(() => MainmenuController.instance.ChangeCharacter());
                logOutButton.onClick.AddListener(() => StartMenuHandler.instance.CloseStartMenu());
                logOutButton.onClick.AddListener(() => MainmenuController.instance.Logout());
            }
            else
            {
                if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
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

        public void OpenChangeTeamMenu()
        {

            List<OptionsPaneOption.OptionsPaneContents> changeTeamOptions = new List<OptionsPaneOption.OptionsPaneContents>();
            Gametype.Teams[] teams = (Gametype.Teams[])Enum.GetValues(typeof(Gametype.Teams));

            List<Gametype.Teams> availableTeams = new List<Gametype.Teams>();

            foreach (Gametype.Teams team in teams)
            {
                if (team != Gametype.Teams.None)
                    availableTeams.Add(team);

            }

            Array.Resize(ref teams, NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.maxTeams - 1);
            foreach(Gametype.Teams availableTeam in availableTeams)
            {
                changeTeamOptions.Add(new OptionsPaneOption.OptionsPaneContents(availableTeam.ToString(), "Switch to the " + availableTeam.ToString() + " team"));
            }

            OptionsPaneHandler.InstanceOpt1ionsPane(GetComponentInParent<Canvas>().gameObject.transform).ShowOptions("Change Team", changeTeamOptions, ChangeTeam, true);
        }

        public void ChangeTeam(string option)
        {
            PlayerData.localPlayerData.CmdChangeTeam((Gametype.Teams)Enum.Parse(typeof(Gametype.Teams), option));
        }

        public void ChangeTeamHover()
        {
            optionImage.sprite = changeTeamSprite;
            optionText.text =
                "Change your team, fight for the other side.";
        }

        public void NoHover()
        {
            optionImage.sprite = Scenario.GetMapImage(Scenario.instance.currentScene);
            if(Scenario.instance.currentGametype == Scenario.Gametype.Ui)
                optionText.text = Scenario.GetMapTitle(Scenario.instance.currentScene);
            else
                optionText.text = Scenario.instance.currentGametype + " on " + Scenario.GetMapTitle(Scenario.instance.currentScene);
        }

        public void LeaveGameHover()
        {
            optionImage.sprite = leaveGameSprite;
            optionText.text =
                "Leave the game and return to the main menu.";
        }

        public void EndGameHover()
        {
            optionImage.sprite = endGameSprite;
            optionText.text =
                "End the game and take your lobby back to the main menu.";
;        }

        public void ChangeCharacterHover()
        {
            optionImage.sprite = changeCharacterSprite;
            optionText.text =
                "Go back to the character selection screen.";
        }

        public void LogOutHover()
        {
            optionImage.sprite = logOutSprite;
            optionText.text =
                "Return to the login screen.";
        }

        //Debug Button

#if !UNITY_EDITOR
        private void Start()
        {
            debugButton.gameObject.SetActive(false);
        }
#else

        public void DebugHover()
        {
            optionImage.sprite = changeCharacterSprite;
            optionText.text =
                "No Function.";
        }

        public void DebugClick()
        {
            //Components.LobbyHandler.PlayerNameplate newPlate = new Components.LobbyHandler.PlayerNameplate("Test Nameplate", false, false, false, Session.activeCharacter);
            //Components.LobbyHandler.AddPlayer(newPlate);
        }
#endif
    }
}
