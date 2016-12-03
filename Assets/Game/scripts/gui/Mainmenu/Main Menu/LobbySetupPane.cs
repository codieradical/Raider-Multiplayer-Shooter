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
            NetworkManager.instance.lobbySetup.scenarioGametype = gametype;

            if (Scenario.instance.getScenesByGametype(gametype).Count < 1)
                NetworkManager.instance.lobbySetup.SelectedScene = "No Scenes Found for " + gametype;
            else
                NetworkManager.instance.lobbySetup.SelectedScene = Scenario.instance.getScenesByGametype(gametype)[0];

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

            if (NetworkManager.instance.currentNetworkState != NetworkManager.NetworkState.Offline)
                NetworkManager.instance.currentNetworkState = NetworkManager.NetworkState.Offline;
        }

        public void StartGame()
        {
            if (NetworkManager.instance.currentNetworkState == NetworkManager.NetworkState.Offline)
                Scenario.instance.LoadScene(NetworkManager.instance.lobbySetup.SelectedScene, NetworkManager.instance.lobbySetup.scenarioGametype);
            else
                NetworkManager.instance.ReadyToBegin();
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
            paneTitle.text = NetworkManager.instance.lobbySetup.Gametype;
            mapLabel.text = NetworkManager.instance.lobbySetup.SelectedScene;
            networkLabel.text = NetworkManager.instance.lobbySetup.Network;
            mapImage.sprite = Scenario.GetMapImage(NetworkManager.instance.lobbySetup.SelectedScene);

            startGameButton.enabled = NetworkManager.instance.isLeader;
        }
    }
}