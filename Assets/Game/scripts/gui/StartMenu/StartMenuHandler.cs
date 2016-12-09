using Raider.Game.Networking;
using Raider.Game.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
    [RequireComponent(typeof(Animator))]
    public class StartMenuHandler : MonoBehaviour
    {
        #region Singleton Setup

        public StartMenuHandler instance;
        Animator animatorInstance;

        // Use this for initialization
        void Awake()
        {
            if (instance != null)
                Debug.LogWarning("More than one StartMenuHandler instance");
            instance = this;
            animatorInstance = GetComponent<Animator>();

            DontDestroyOnLoad(transform.parent);
        }

        void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public bool IsOpen
        {
            get { return animatorInstance.GetBool("open"); }
            set { animatorInstance.SetBool("open", value); }
        }

        public Text usernameLabel;
        public Text gametypeLabel;
        public Button leaveGameButton;
        public Text leaveGameText;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (!IsOpen)
                    OpenStartMenu();
                else
                    CloseStartMenu();
        }

        public void OpenStartMenu()
        {
            //If the user is not logged in, don't let them do this.
            if (Session.activeCharacter == null)
                return;

            SetupStartMenuData();

            if (!Scenario.InLobby)
                Player.Player.localPlayer.PausePlayer();

            animatorInstance.SetBool("open", true);
        }

        void SetupStartMenuData()
        {
            usernameLabel.text = Session.saveDataHandler.GetUsername();

            leaveGameButton.onClick.RemoveAllListeners();
            leaveGameButton.onClick.AddListener(CloseStartMenu);
            leaveGameButton.onClick.AddListener(Scenario.instance.LeaveGame);

            if (Scenario.InLobby)
            {
                leaveGameButton.gameObject.SetActive(false);
                gametypeLabel.text = "Mainmenu";
            }
            else
            {
                gametypeLabel.text = NetworkManager.instance.lobbySetup.GametypeString;
                leaveGameButton.gameObject.SetActive(true);

                if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Client)
                    leaveGameText.text = "Leave Game";
                else
                    leaveGameText.text = "End Game";
            }
        }

        public void CloseStartMenu()
        {
            if (!Scenario.InLobby)
                Player.Player.localPlayer.UnpausePlayer();

            animatorInstance.SetBool("open", false);
        }
    }
}