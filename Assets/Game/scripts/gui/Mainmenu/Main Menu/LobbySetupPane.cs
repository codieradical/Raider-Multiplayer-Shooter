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
            NetworkManager.instance.lobbySetup.Gametype = gametype;

            NetworkManager.instance.lobbySetup.Network = "Offline";

            if (Scenario.instance.GetSceneNamesByGametype(gametype).Count < 1)
                NetworkManager.instance.lobbySetup.SelectedScene = "No Scenes Found for " + gametype;
            else
                NetworkManager.instance.lobbySetup.SelectedScene = Scenario.instance.GetSceneNamesByGametype(gametype)[0];

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
            NetworkManager.instance.actionQueue.Enqueue(UpdatePaneData);
        }

        public void ClosePane()
        {
            animatorInstance.SetBool("open", false);
            GametypeButtons.instance.ShowButtons();

            if (NetworkManager.instance.CurrentNetworkState != NetworkManager.NetworkState.Offline)
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
        }

        public void StartGame()
        {
            if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Offline)
                Scenario.instance.LoadScene(NetworkManager.instance.lobbySetup.SelectedScene, NetworkManager.instance.lobbySetup.Gametype);
            else
            {
                Scenario.instance.currentScene = NetworkManager.instance.lobbySetup.SelectedScene;
                Scenario.instance.currentGametype = NetworkManager.instance.lobbySetup.Gametype;
                NetworkManager.instance.ReadyToBegin();
            }
            
        }

        // Use this for initialization
        void Start()
        {
            if (instance != null)
                Debug.LogWarning("Multiple LobbySetupPanes instanced");
            instance = this;

            //This could use tidying.
            if (paneTitle == null || mapLabel == null || networkLabel == null || gametypeLabel == null || mapImage == null)
                Debug.LogWarning("The lobby setup pane is missing some important objects!");

            animatorInstance = GetComponent<Animator>();
        }

        void OnDestroy()
        {
            instance = null;
        }

        public void UpdatePaneData()
        {
            paneTitle.text = NetworkManager.instance.lobbySetup.GametypeString;
            mapLabel.text = NetworkManager.instance.lobbySetup.SelectedScene;
            networkLabel.text = NetworkManager.instance.lobbySetup.Network;
            mapImage.sprite = Scenario.GetMapImage(NetworkManager.instance.lobbySetup.SelectedScene);

            startGameButton.gameObject.SetActive(NetworkManager.instance.IsLeader);
        }

        public void OpenMapOptions()
        {
            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            foreach(string scene in Scenario.instance.GetSceneNamesByGametype(NetworkManager.instance.lobbySetup.Gametype))
            {
                options.Add(new OptionsPaneOption.OptionsPaneContents(scene, "This is the scene description", Scenario.GetMapImage(scene)));
            }

            OptionsPaneHandler.instance.ShowOptions("Select Map...", options, SelectMap);
        }

        public void SelectMap(string mapName)
        {
            NetworkManager.instance.lobbySetup.SelectedScene = mapName;
        }
    }
}