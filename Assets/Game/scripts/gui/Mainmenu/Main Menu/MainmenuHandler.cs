using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using Raider.Game.GUI.Components;
using Raider.Game.Cameras;

namespace Raider.Game.GUI.Screens
{

    [RequireComponent(typeof(MenuManager))]
    public class MainmenuHandler : MonoBehaviour
    {

        #region Singleton Setup

        public static MainmenuHandler instance;

        public void Awake()
        {
            if (instance != null)
                Debug.LogAssertion("It seems that multiple Main Menu Managers are active, breaking the singleton instance.");
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion


        //Not sure where this could go...
        public void SetupCameraMode()
        {
            CameraModeController.singleton.cameraPathGameObject = GameObject.Find("_CameraPath");
            CameraModeController.singleton.selectedCameraMode = CameraModeController.CameraModes.FollowPath;
        }

        public void Start()
        {
            SetupCameraMode();
            if (Session.activeCharacter != null)
            {
                MenuManager.instance.ShowMenu(MainMenuScreen.GetComponent<Menu>());

                LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate();
                playerNameplate.username = Session.saveDataHandler.GetUsername();
                playerNameplate.leader = true;
                playerNameplate.character = Session.activeCharacter;
                LobbyHandler.AddPlayer(playerNameplate);

                GametypeButtons.instance.ShowButtons();
            }
        }

        [SerializeField]
        public CharacterEditorHandler editorHandler;

        [Header("Screens")]
        public GameObject LoginScreen;
        public GameObject ChooseCharacterScreen;
        public GameObject CreateCharacterScreen;
        public GameObject MainMenuScreen;
        public GameObject EmblemEditorScreen;
        public GameObject MatchmakingScreen;

        public void Login(Text _textComponent)
        {
            string _username = _textComponent.text;

            if (string.IsNullOrEmpty(_username))
            {
                UserFeedback.LogError("No username provided.");
                //Display info.
                return;
            }

            Session.Login(_username);
            MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
            ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
        }

        public void CloseCharacterEditor()
        {
            MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
            ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
        }

        public void ChooseCharacter(int characterIndex)
        {
            Session.SelectCharacter(characterIndex);

            LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate();
            playerNameplate.username = Session.saveDataHandler.GetUsername();
            playerNameplate.leader = true;
            playerNameplate.character = Session.activeCharacter;

            LobbyHandler.AddPlayer(playerNameplate);

            MenuManager.instance.ShowMenu(MainMenuScreen.GetComponent<Menu>());
            GametypeButtons.instance.ShowButtons();
        }

        public void CreateCharacter()
        {
            CreateCharacterScreen.GetComponent<CharacterEditorHandler>().NewCharacter();
            MenuManager.instance.ShowMenu(CreateCharacterScreen.GetComponent<Menu>());
        }

        public void EditCharacter(int slot)
        {
            CreateCharacterScreen.GetComponent<CharacterEditorHandler>().EditCharacter(slot);
            MenuManager.instance.ShowMenu(CreateCharacterScreen.GetComponent<Menu>());
        }
    }
}