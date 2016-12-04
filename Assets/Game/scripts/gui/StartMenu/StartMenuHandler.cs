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

        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
                if (!IsOpen)
                    OpenStartMenu();
                else
                    CloseStartMenu();
        }

        public void OpenStartMenu()
        {
            animatorInstance.SetBool("open", true);
            //If the user is not logged in, don't let them do this.
            if (Session.activeCharacter == null)
                return;

            if (!Scenario.inLobby)
                Player.Player.localPlayer.PausePlayer();

            usernameLabel.text = Session.saveDataHandler.GetUsername();

            if (Scenario.inLobby)
                gametypeLabel.text = "Mainmenu";
            else
                gametypeLabel.text = NetworkManager.instance.lobbySetup.Gametype;
        }

        public void CloseStartMenu()
        {
            if (!Scenario.inLobby)
                Player.Player.localPlayer.UnpausePlayer();

            animatorInstance.SetBool("open", false);
        }
    }
}