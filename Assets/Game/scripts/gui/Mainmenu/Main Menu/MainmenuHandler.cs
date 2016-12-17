using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using Raider.Game.GUI.Components;
using Raider.Game.Cameras;
using Raider.Game.Networking;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Saves;

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

        public void Start()
        {
            if (Session.activeCharacter != null)
            {
                MenuManager.instance.ShowMenu(MainMenuScreen.GetComponent<Menu>());
                if (NetworkManager.instance.CurrentNetworkState != NetworkManager.NetworkState.Offline)
                {
                    NetworkManager.instance.UpdateLobbyNameplates();
                    LobbySetupPane.instance.OpenPane();
                }
                else
                {
                    LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate()
                    {
                        username = Session.saveDataHandler.GetUsername(),
                        leader = true,
                        character = Session.activeCharacter
                    };

                    //Make sure the old player is gone.
                    LobbyHandler.DestroyAllPlayers();
                    LobbyHandler.AddPlayer(playerNameplate);

                    GametypeButtons.instance.ShowButtons();
                }
            }
            else if(Session.saveDataHandler != null)
            {
                MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
                ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
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

            if (Session.saveDataHandler.GetSettings().lobbyDisplay == SaveDataStructure.Settings.LobbyDisplay.Split)
                LobbyHandler.SwitchToSplitLobby();
            else
                LobbyHandler.SwitchToScrollLobby();
        }

        public void Logout()
        {
            Session.Logout();

            if (NetworkManager.instance.CurrentNetworkState != NetworkManager.NetworkState.Offline)
            {
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
                return;
            }

            MenuManager.instance.ShowMenu(LoginScreen.GetComponent<Menu>());
            LobbyHandler.DestroyAllPlayers(); //Make sure to remove the old character from the lobby.
            //Maybe I should move this to the session handler.
        }

        public void CloseCharacterEditor()
        {
            MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
            ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
        }

        public void ChooseCharacter(int characterIndex)
        {
            Session.SelectCharacter(characterIndex);

            //We're done with the plate previews.
            CharacterPreviewHandler.instance.DestroyPreviews();

            MenuManager.instance.ShowMenu(MainMenuScreen.GetComponent<Menu>());
            GametypeButtons.instance.ShowButtons();

            LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate()
            {
                username = Session.saveDataHandler.GetUsername(),
                leader = true,
                character = Session.activeCharacter
            };
            LobbyHandler.AddPlayer(playerNameplate);
        }

        public void ChangeCharacter()
        {
            Session.DeselectCharacter();

            if (NetworkManager.instance.CurrentNetworkState != NetworkManager.NetworkState.Offline)
            {
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
                return;
            }

            LobbyHandler.DestroyAllPlayers(); //Make sure to remove the old character from the lobby.
            //Maybe I should move this to the session handler.
            ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
            MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());

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