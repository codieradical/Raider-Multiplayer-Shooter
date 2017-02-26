using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raider.Game.Scene;
using System.Collections.Generic;
using Raider.Game.Networking;
using System;
using Raider.Game.Player;
using Raider.Game.Gametypes;
using Raider.Game.GUI.Screens;

namespace Raider.Game.GUI.Components
{

    public class LobbySetupPane : MonoBehaviour
    {

        public bool IsOpen
        {
            get { return animatorInstance.GetBool("open"); }
            set { animatorInstance.SetBool("open", value); }
        }

        public static LobbySetupPane instance;

        Animator animatorInstance;

        public Text paneTitle;
        public Text mapLabel;
        public Text networkLabel;
        public Text gametypeLabel;
        public Image mapImage;
        public Button startGameButton;

        public void OpenPane(Scenario.Gametype gametype)
        {
            IsOpen = true;
            NetworkGameManager.instance.lobbySetup.syncData.Gametype = gametype;

            NetworkGameManager.instance.lobbySetup.syncData.Network = "Offline";

            if (Scenario.instance.GetSceneNamesByGametype(gametype).Count < 1)
                NetworkGameManager.instance.lobbySetup.syncData.SelectedScene = "No Scenes Found for " + gametype;
            else
                NetworkGameManager.instance.lobbySetup.syncData.SelectedScene = Scenario.instance.GetSceneNamesByGametype(gametype)[0];

            NetworkGameManager.instance.lobbySetup.syncData.gameOptions = Gametype.GetGameOptionsByEnum(gametype);

            UpdatePaneData();
        }

        //Used to open the pane before data has been send from the server...
        public void OpenPane()
        {
            IsOpen = true;

            paneTitle.text = "Loading Lobby...";
            mapLabel.text = "";
            networkLabel.text = "";
            mapImage.sprite = null;
        }

        public void ClosePane()
        {
            IsOpen = false;
            GametypeButtons.instance.ShowButtons();

            if (NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
        }

        public void StartGame()
        {
            if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Offline)
                Scenario.instance.LoadScene(NetworkGameManager.instance.lobbySetup.syncData.SelectedScene, NetworkGameManager.instance.lobbySetup.syncData.Gametype);
            else
            {
                Scenario.instance.currentScene = NetworkGameManager.instance.lobbySetup.syncData.SelectedScene;
                Scenario.instance.currentGametype = NetworkGameManager.instance.lobbySetup.syncData.Gametype;
                foreach (PlayerData player in NetworkGameManager.instance.Players)
                {
                    if (!player.syncData.GotData)
                    {
                        UserFeedback.LogError("A player is still connecting. Please wait until all players have joined.");
                        return;
                    }
                }
                NetworkGameManager.instance.ReadyToBegin();
            }
            
        }

        void Awake()
        {
            if (instance != null)
                Debug.LogWarning("Multiple LobbySetupPanes instanced");
            instance = this;

            animatorInstance = GetComponent<Animator>();
        }

        // Use this for initialization
        void Start()
        {
            //This could use tidying.
            if (paneTitle == null || mapLabel == null || networkLabel == null || gametypeLabel == null || mapImage == null)
                Debug.LogWarning("The lobby setup pane is missing some important objects!");
        }

        void OnDestroy()
        {
            instance = null;
        }

        public void UpdatePaneData()
        {
            paneTitle.text = NetworkGameManager.instance.lobbySetup.syncData.GametypeString;
            mapLabel.text = NetworkGameManager.instance.lobbySetup.syncData.SelectedScene;
            networkLabel.text = NetworkGameManager.instance.lobbySetup.syncData.Network;
            mapImage.sprite = Scenario.GetMapImage(NetworkGameManager.instance.lobbySetup.syncData.SelectedScene);

            startGameButton.gameObject.SetActive(NetworkGameManager.instance.IsLeader);
        }

        public void OpenGameOptions()
        {
            if (!NetworkGameManager.instance.IsLeader)
                return;

            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            options.Add(new OptionsPaneOption.OptionsPaneContents("Score To Win: " + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin.ToString(), "The amount of points required to win."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Teams Enabled", "Enable or Disable teams."));
            options.Add(new OptionsPaneOption.OptionsPaneContents(NetworkGameManager.instance.lobbySetup.syncData.Gametype + " Options", "Change the " + NetworkGameManager.instance.lobbySetup.syncData.Gametype + " options"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Team Options", "Change team gameplay options, if teams are enabled."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("General Options", "Change general game options."));

            OptionsPaneHandler.InstanceOpt1ionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Game Options", options, SelectGameOptions, false);
        }
        
        public void SelectGameOptions(string option)
        {
            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();
            if (option.Contains("Score To Win"))
            {
                options.Add(new OptionsPaneOption.OptionsPaneContents("25", "25 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("50", "50 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("75", "75 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("100", "100 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("125", "125 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("150", "150 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("175", "175 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("200", "200 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("225", "225 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("250", "250 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("300", "300 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("400", "400 points to win."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("500", "500 points to win."));

                OptionsPaneHandler.InstanceOpt1ionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Game Options", options, SelectScoreToWin, true);
            }

            if (option.Contains("Teams Enabled"))
            {
                options.Add(new OptionsPaneOption.OptionsPaneContents("Enabled", "Allow players to fight as a team."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("Disabled", "Free for all gameplay."));

                OptionsPaneHandler.InstanceOpt1ionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Game Options", options, SelectTeamsEnabled, true);
            }
        }

        public void SelectTeamsEnabled(string option)
        {
            if(option == "Enabled" || option == "Disabled")
            {
                OptionsPaneHandler[] oldOpenPanes = OptionsPaneHandler.GetOptionsPanes();

                if(option == "Enabled")
                    NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled = true;
                else
                    NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled = false;

                NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(NetworkGameManager.instance.lobbySetup.syncData);

                OpenGameOptions();

                foreach (OptionsPaneHandler optionsPane in oldOpenPanes)
                {
                    Destroy(optionsPane.gameObject);
                }
            }
        }

        public void SelectScoreToWin(string option)
        {
            int selectedValue;
            if(int.TryParse(option, out selectedValue))
            {
                OptionsPaneHandler[] oldOpenPanes = OptionsPaneHandler.GetOptionsPanes();

                NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin = selectedValue;
                NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(NetworkGameManager.instance.lobbySetup.syncData);

                OpenGameOptions();

                foreach(OptionsPaneHandler optionsPane in oldOpenPanes)
                {
                    Destroy(optionsPane.gameObject);
                }
            }
        }

        public void OpenMapOptions()
        {
            if (!NetworkGameManager.instance.IsLeader)
                return;

            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            foreach(string scene in Scenario.instance.GetSceneNamesByGametype(NetworkGameManager.instance.lobbySetup.syncData.Gametype))
            {
                options.Add(new OptionsPaneOption.OptionsPaneContents(scene, Scenario.GetMapDescription(scene), Scenario.GetMapImage(scene)));
            }

            OptionsPaneHandler.InstanceOpt1ionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Select Map...", options, SelectMap, true);
        }

        public void SelectMap(string mapName)
        {
            NetworkGameManager.instance.lobbySetup.syncData.SelectedScene = mapName;
        }

        public void OpenNetworkOptions()
        {
            if (!NetworkGameManager.instance.IsLeader)
                return;

            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            options.Add(new OptionsPaneOption.OptionsPaneContents("Offline", "Splitscreen co-op. Not Yet Implemented."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Local", "Host a Local Area Nework Lobby. Not Yet Implemented."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Online", "Host an online lobby on your PC"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Online Server", "Host an online server lobby on your PC"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Matchmaker/Dedicated", "Not yet implemented."));

            OptionsPaneHandler.InstanceOpt1ionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Network", options, SelectNetwork, true);
        }

        public void SelectNetwork(string option)
        {
            NetworkGameManager.instance.lobbySetup.syncData.Network = option;
            //The user might be switching from host to server, so it's important to end communications first.
            if (option == "Online")
            {
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Host;
            }

            if (option == "Online Server")
            {
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Server;
            }

        }
    }
}