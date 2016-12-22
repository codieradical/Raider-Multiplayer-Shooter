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
            animatorInstance.SetBool("open", true);
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
            animatorInstance.SetBool("open", true);
            paneTitle.text = "Loading Lobby...";
            mapLabel.text = "";
            networkLabel.text = "";
            mapImage.sprite = null;
            NetworkGameManager.instance.actionQueue.Enqueue(UpdatePaneData);
        }

        public void ClosePane()
        {
            animatorInstance.SetBool("open", false);
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
            //If the player is the leader, allow them to interact with options.
            //Else, don't.
            if (NetworkGameManager.instance.IsLeader)
                GetComponent<CanvasGroup>().interactable = GetComponent<CanvasGroup>().blocksRaycasts = true;
            else
                GetComponent<CanvasGroup>().interactable = GetComponent<CanvasGroup>().blocksRaycasts = false;

            paneTitle.text = NetworkGameManager.instance.lobbySetup.GametypeString;
            mapLabel.text = NetworkGameManager.instance.lobbySetup.SelectedScene;
            networkLabel.text = NetworkGameManager.instance.lobbySetup.Network;
            mapImage.sprite = Scenario.GetMapImage(NetworkGameManager.instance.lobbySetup.SelectedScene);

            startGameButton.gameObject.SetActive(NetworkGameManager.instance.IsLeader);
        }

        public void OpenMapOptions()
        {
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
    }
}