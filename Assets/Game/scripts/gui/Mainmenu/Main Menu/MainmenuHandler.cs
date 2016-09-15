using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using Raider.Game.GUI.Components;

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


        [SerializeField]
        public CharacterEditorHandler editorHandler = new CharacterEditorHandler();

        [Header("Screens")]
        public GameObject LoginScreen;
        public GameObject ChooseCharacterScreen;
        public GameObject CreateCharacterScreen;
        public GameObject MainMenuScreen;

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
            ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
            MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
        }

        public void CloseCharacterEditor()
        {
            ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
            MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
        }

        public void ChooseCharacter(int characterIndex)
        {
            Session.SelectCharacter(characterIndex);

            LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate();
            playerNameplate.username = Session.saveDataHandler.GetUsername();
            playerNameplate.leader = true;
            playerNameplate.character = Session.character;

            LobbyHandler.AddPlayer(playerNameplate);

            MenuManager.instance.ShowMenu(MainMenuScreen.GetComponent<Menu>());
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