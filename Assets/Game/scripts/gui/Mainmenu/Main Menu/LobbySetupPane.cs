using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raider.Game.Scene;
using System.Collections.Generic;
using Raider.Game.Networking;
using System;

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
            NetworkGameManager.instance.lobbySetup.Gametype = gametype;

            NetworkGameManager.instance.lobbySetup.Network = "Offline";

            if (Scenario.instance.GetSceneNamesByGametype(gametype).Count < 1)
                NetworkGameManager.instance.lobbySetup.SelectedScene = "No Scenes Found for " + gametype;
            else
                NetworkGameManager.instance.lobbySetup.SelectedScene = Scenario.instance.GetSceneNamesByGametype(gametype)[0];

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

        public void StartGame()
        {
            if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Offline)
                Scenario.instance.LoadScene(NetworkGameManager.instance.lobbySetup.SelectedScene, NetworkGameManager.instance.lobbySetup.Gametype);
            else
            {
                Scenario.instance.currentScene = NetworkGameManager.instance.lobbySetup.SelectedScene;
                Scenario.instance.currentGametype = NetworkGameManager.instance.lobbySetup.Gametype;
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
            paneTitle.text = NetworkGameManager.instance.lobbySetup.GametypeString;
            mapLabel.text = NetworkGameManager.instance.lobbySetup.SelectedScene;
            networkLabel.text = NetworkGameManager.instance.lobbySetup.Network;
            mapImage.sprite = Scenario.GetMapImage(NetworkGameManager.instance.lobbySetup.SelectedScene);

            startGameButton.gameObject.SetActive(NetworkGameManager.instance.IsLeader);
        }

        public void OpenMapOptions()
        {
            if (!NetworkGameManager.instance.IsLeader)
                return;

            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            foreach(string scene in Scenario.instance.GetSceneNamesByGametype(NetworkGameManager.instance.lobbySetup.Gametype))
            {
                options.Add(new OptionsPaneOption.OptionsPaneContents(scene, "This is the scene description", Scenario.GetMapImage(scene)));
            }

            OptionsPaneHandler.instance.ShowOptions("Select Map...", options, SelectMap);
        }

        public void SelectMap(string mapName)
        {
            NetworkGameManager.instance.lobbySetup.SelectedScene = mapName;
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

            OptionsPaneHandler.instance.ShowOptions("Network", options, SelectNetwork);
        }

        public void SelectNetwork(string option)
        {
            NetworkGameManager.instance.lobbySetup.Network = option;
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