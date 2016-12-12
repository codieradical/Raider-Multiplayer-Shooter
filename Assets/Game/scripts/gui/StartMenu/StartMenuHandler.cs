using Raider.Game.Networking;
using Raider.Game.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.StartMenu
{
    [RequireComponent(typeof(Animator))]
    public class StartMenuHandler : MonoBehaviour
    {
        #region Singleton Setup

        public static StartMenuHandler instance;
        Animator animatorInstance;

        // Use this for initialization
        void Awake()
        {
            if (instance != null)
                Debug.LogWarning("More than one StartMenuHandler instance");
            instance = this;
            animatorInstance = GetComponent<Animator>();
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

        [Header("Start Menu Panes")]
        public StartMenuGame startMenuGame;
        public StartMenuLobby startMenuLobby;
        public StartMenuPlayer startMenuPlayer;
        public StartMenuSettings startMenuSettings;

        private StartMenuPane defaultPane
        {
            get
            {
                if (Scenario.InLobby)
                {
                    return startMenuPlayer;
                }
                else
                    return startMenuGame;
            }
        }

        private StartMenuPane activePane;

        public Text usernameLabel;
        public Text gametypeLabel;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (!IsOpen)
                    OpenStartMenu();
                else
                    CloseStartMenu();
        }

        void CloseActivePane()
        {
            if (activePane != null)
            {
                activePane.ClosePane();
                activePane.enabled = false;
            }
        }

        public void OpenAPane(StartMenuPane newPane)
        {
            CloseActivePane();
            activePane = newPane;
            newPane.enabled = true;
            newPane.OpenPane();
        }

        public void OpenStartMenu()
        {
            //If the user is not logged in, don't let them do this.
            if (Session.activeCharacter == null)
                return;

            SetupStartMenuData();

            if (!Scenario.InLobby)
                Player.Player.localPlayer.PausePlayer();

            OpenAPane(defaultPane);

            IsOpen = true;
        }

        void SetupStartMenuData()
        {
            usernameLabel.text = Session.saveDataHandler.GetUsername();

            if (Scenario.InLobby)
                gametypeLabel.text = "Mainmenu";
            else
                gametypeLabel.text = NetworkManager.instance.lobbySetup.GametypeString;
        }

        public void CloseStartMenu()
        {
            if (!Scenario.InLobby)
                Player.Player.localPlayer.UnpausePlayer();

            CloseActivePane();

            IsOpen = false;
        }
    }
}