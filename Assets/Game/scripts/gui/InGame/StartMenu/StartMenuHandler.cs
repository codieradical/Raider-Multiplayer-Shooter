using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Scene;
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

        private StartMenuPane DefaultPane
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

        void CloseActivePane()
        {
            if (activePane != null)
            {
                activePane.ClosePane();
                activePane.enabled = false;
                activePane = null;
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
            if (Session.ActiveCharacter == null)
                return;

            SetupStartMenuData();

            if (!Scenario.InLobby)
                PlayerData.localPlayerData.gamePlayerController.PausePlayer();

            OpenAPane(DefaultPane);

            IsOpen = true;
        }

        void SetupStartMenuData()
        {
            usernameLabel.text = Session.userSaveDataHandler.GetUsername();

            if (Scenario.InLobby)
                gametypeLabel.text = "Mainmenu";
            else
                gametypeLabel.text = NetworkGameManager.instance.lobbySetup.syncData.GametypeString;
        }

        public void CloseStartMenu()
        {
            if (!Scenario.InLobby)
                PlayerData.localPlayerData.gamePlayerController.UnpausePlayer();

            CloseActivePane();

            IsOpen = false;
        }
    }
}