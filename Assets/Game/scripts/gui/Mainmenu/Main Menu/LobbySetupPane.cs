using Raider.Game.Gametypes;
using Raider.Game.GUI.Screens;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Scene;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{

	public class LobbySetupPane : MonoBehaviour
    {

        public bool IsOpen
        {
            get { if (animatorInstance != null) return animatorInstance.GetBool("open"); else return false; }
            set { if(animatorInstance != null) animatorInstance.SetBool("open", value); }
        }

        public static LobbySetupPane instance;

        Animator animatorInstance;

        public Text paneTitle;
        public Text mapLabel;
        public Text networkLabel;
        public Text gametypeLabel;
        public Image mapImage;
        public Button startGameButton;

        public void OpenPane(GametypeHelper.Gametype gametype)
        {
            IsOpen = true;

            NetworkGameManager.instance.lobbySetup.syncData.Gametype = gametype;

            NetworkGameManager.instance.lobbySetup.syncData.Network = "Online";

            if (Scenario.instance.GetSceneNamesByGametype(gametype).Count < 1)
                NetworkGameManager.instance.lobbySetup.syncData.SelectedScene = "No Scenes Found for " + gametype;
            else
                NetworkGameManager.instance.lobbySetup.syncData.SelectedScene = Scenario.instance.GetSceneNamesByGametype(gametype)[0];

            NetworkGameManager.instance.lobbySetup.syncData.gameOptions = GametypeController.GetGameOptionsByEnum(gametype);

            //Update teams.
            if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
            {
                for (int i = NetworkGameManager.instance.Players.Count - 1; i >= 0; i--)
                {
                    if (NetworkGameManager.instance.Players[i].PlayerSyncData.team == GametypeHelper.Team.None)
                    {
                        if (i % 2 == 0)
                        {
                            NetworkGameManager.instance.Players[i].PlayerSyncData.team = GametypeHelper.Team.Red;
                            NetworkGameManager.instance.Players[i].RpcChangeTeam(GametypeHelper.Team.Red);
                        }
                        else
                        {
                            NetworkGameManager.instance.Players[i].PlayerSyncData.team = GametypeHelper.Team.Blue;
                            NetworkGameManager.instance.Players[i].RpcChangeTeam(GametypeHelper.Team.Blue);
                        }
                    }
                }
            }
            else
            {
                foreach (PlayerData player in NetworkGameManager.instance.Players)
                {
                    if (player.PlayerSyncData.team != GametypeHelper.Team.None)
                    {
                        player.PlayerSyncData.team = GametypeHelper.Team.None;
                        player.RpcChangeTeam(GametypeHelper.Team.None);
                    }
                }
            }

            NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
            NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Host;

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
            NetworkGameManager.instance.actionQueue.Enqueue(UpdatePaneData);
        }

        public void ClosePane()
        {
            IsOpen = false;
            GametypeButtons.instance.ShowButtons();

            if (NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
        }

        public void ClosePaneNextFrame()
        {
            NetworkGameManager.instance.actionQueue.Enqueue(ClosePane);
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
                    if (!player.PlayerSyncData.GotData)
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

            NetworkGameManager.instance.onClientDisconnect += ClosePaneNextFrame;
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
            if (NetworkGameManager.instance.lobbySetup == null || NetworkGameManager.instance.lobbySetup.syncData == null)
                return;

            paneTitle.text = NetworkGameManager.instance.lobbySetup.syncData.GametypeString;
            mapLabel.text = Scenario.GetMapTitle(NetworkGameManager.instance.lobbySetup.syncData.SelectedScene);
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
            options.Add(new OptionsPaneOption.OptionsPaneContents("Teams Enabled: " + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled.ToString(), "Enable or Disable teams."));
            options.Add(new OptionsPaneOption.OptionsPaneContents(NetworkGameManager.instance.lobbySetup.syncData.Gametype + " Options", "Change the " + NetworkGameManager.instance.lobbySetup.syncData.Gametype + " options"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Team Options", "Change team gameplay options, if teams are enabled."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("General Options", "Change general game options."));

            OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Game Options", options, SelectGameOptions, false);
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

                OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Game Options", options, SelectScoreToWin, true);
            }

            if (option.Contains("Teams Enabled"))
            {
				if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.forceTeams)
					return;

                options.Add(new OptionsPaneOption.OptionsPaneContents("Enabled", "Allow players to fight as a team."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("Disabled", "Free for all gameplay."));

                OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Game Options", options, SelectTeamsEnabled, true);
            }

			if (option.Contains("General Options"))
			{
				options.Add(new OptionsPaneOption.OptionsPaneContents("Number Of Rounds: " + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.numberOfRounnds.ToString(), "How many rounds will take place before the game ends."));
                if(!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.TimeLimit)
                    options.Add(new OptionsPaneOption.OptionsPaneContents("Time Limit: None", "How long a round lasts."));
                else
                    options.Add(new OptionsPaneOption.OptionsPaneContents("Time Limit: " + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.timeLimitMinutes.ToString(), "How long a round lasts."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("Respawn Time: " + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds.ToString(), "How long players must wait to respawn upon death."));
				OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("General Options", options, SelectGeneralOptions, true);
			}
		}

		public void SelectGeneralOptions(string option)
		{
			List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();
			if (option.Contains("Respawn Time"))
			{
				options.Add(new OptionsPaneOption.OptionsPaneContents("0 Seconds", "Instant Respawn"));
				options.Add(new OptionsPaneOption.OptionsPaneContents("3 Seconds", "3 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("5 Seconds", "5 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("6 Seconds", "6 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("7 Seconds", "7 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("8 Seconds", "8 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("9 Seconds", "9 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("10 Seconds", "10 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("15 Seconds", "15 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("20 Seconds", "20 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("30 Seconds", "30 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("45 Seconds", "45 seconds to respawn."));
				options.Add(new OptionsPaneOption.OptionsPaneContents("60 Seconds", "60 seconds to respawn."));

				OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Respawn Time", options, SelectRespawnTime, true);
			}
            else if (option.Contains("Time Limit"))
            {
                options.Add(new OptionsPaneOption.OptionsPaneContents("None", "No time limit."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("3 Minutes", "Rounds last 3 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("5 Minutes", "Rounds last 5 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("10 Minutes", "Rounds last 10 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("15 Minutes", "Rounds last 15 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("20 Minutes", "Rounds last 20 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("25 Minutes", "Rounds last 25 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("30 Minutes", "Rounds last 30 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("40 Minutes", "Rounds last 40 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("45 Minutes", "Rounds last 45 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("50 Minutes", "Rounds last 50 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("60 Minutes", "Rounds last 60 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("90 Minutes", "Rounds last 90 minutes."));
                options.Add(new OptionsPaneOption.OptionsPaneContents("120 Minutes", "Rounds last 120 minutes."));

                OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Time Limit", options, SelectTimeLimit, true);
            }
        }

        //Refactor me!
        public void SelectTimeLimit(string option)
        {
            int value;

            if (option == "None")
            {
                value = -1;

                OptionsPaneHandler[] oldOpenPanes = OptionsPaneHandler.GetOptionsPanes();
                NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.timeLimitMinutes = value;
                NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(NetworkGameManager.instance.lobbySetup.syncData);

                OpenGameOptions();

                foreach (OptionsPaneHandler optionsPane in oldOpenPanes)
                {
                    Destroy(optionsPane.gameObject);
                }
            }
            else if (int.TryParse(option.Replace(" Minutes", ""), out value))
            {
                OptionsPaneHandler[] oldOpenPanes = OptionsPaneHandler.GetOptionsPanes();
                NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.timeLimitMinutes = value;
                NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(NetworkGameManager.instance.lobbySetup.syncData);

                OpenGameOptions();

                foreach (OptionsPaneHandler optionsPane in oldOpenPanes)
                {
                    Destroy(optionsPane.gameObject);
                }
            }
        }

        public void SelectRespawnTime(string option)
		{
			int value;

			if (int.TryParse(option.Replace(" Seconds", ""), out value))
			{
				OptionsPaneHandler[] oldOpenPanes = OptionsPaneHandler.GetOptionsPanes();
				NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds = value;
				NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(NetworkGameManager.instance.lobbySetup.syncData);

				OpenGameOptions();

				foreach (OptionsPaneHandler optionsPane in oldOpenPanes)
				{
					Destroy(optionsPane.gameObject);
				}
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
                options.Add(new OptionsPaneOption.OptionsPaneContents(Scenario.GetMapTitle(scene), Scenario.GetMapDescription(scene), Scenario.GetMapImage(scene)));
            }

            OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Select Map...", options, SelectMap, true);
        }

        public void SelectMap(string title)
        {
            NetworkGameManager.instance.lobbySetup.syncData.SelectedScene = Scenario.GetMapNameFromTitle(title);
        }

		//REMOVED FEATURE
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

            OptionsPaneHandler.InstanceOptionsPane(MainmenuController.instance.MainMenuScreen.transform).ShowOptions("Network", options, SelectNetwork, true);
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