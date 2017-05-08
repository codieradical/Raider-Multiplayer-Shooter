using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.GUI.Components;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Saves.User;
using Raider.Game.Cameras;
using UnityEngine;

namespace Raider.Game.GUI.Screens
{

    [RequireComponent(typeof(MenuManager))]
    public class MainmenuController : MonoBehaviour
    {

        #region Singleton Setup

        public static MainmenuController instance;

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
			if (animatorController != null)
			{
				CameraModeController.singleton.animatorController = animatorController;
				CameraModeController.singleton.SetCameraMode(CameraModeController.CameraModes.Animated);
			}

            if (Session.ActiveCharacter != null)
            {
                MenuManager.instance.ShowMenu(MainMenuScreen.GetComponent<Menu>());
                if (NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
                {
                    NetworkGameManager.instance.UpdateLobbyNameplates();
                    LobbySetupPane.instance.OpenPane();
                }
                else
                {
                    PlayerData.SyncData playerSyncData = new PlayerData.SyncData()
                    {
                        Character = Session.ActiveCharacter,
                        username = Session.userSaveDataHandler.GetUsername(),
                        isLeader = true,
                        id = 0
                    };

                    //Make sure the old player is gone.
                    LobbyHandler.DestroyAllPlayers();
                    LobbyHandler.AddPlayer(playerSyncData);

                    GametypeButtons.instance.ShowButtons();
                }
            }
            else if(Session.userSaveDataHandler != null)
            {
                MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
                ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
            }
        }

		[Header("Camera Setup")]
		public RuntimeAnimatorController animatorController;

        [SerializeField]
        public CharacterEditorHandler editorHandler;

        [Header("Screens")]
        public GameObject LoginScreen;
        public GameObject ChooseCharacterScreen;
        public GameObject CreateCharacterScreen;
        public GameObject MainMenuScreen;
        public GameObject EmblemEditorScreen;
        public GameObject MatchmakingScreen;

        [Header("Prefabs")]
        public Object OptionsPanePrefab;

        public void Login()
        {
            //If the player has no characters, skip character selection, go straight to creation.
            if(Session.userSaveDataHandler.GetAllCharacters().Count < 1)
            {
                CreateCharacter();
            }
            else
            {
                ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
                MenuManager.instance.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
            }

            if (Session.userSaveDataHandler.GetSettings().LobbyDisplay == UserSaveDataStructure.UserSettings.LobbyDisplays.Split)
                LobbyHandler.SwitchToSplitLobby();
            else
                LobbyHandler.SwitchToScrollLobby();
        }

        public void Logout()
        {
            Session.Logout();

            if (NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
            {
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
                return;
            }

            LoginScreen.GetComponent<LoginHandler>().ResetMenu();
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

            PlayerData.SyncData playerSyncData = new PlayerData.SyncData()
            {
                Character = Session.ActiveCharacter,
                username = Session.userSaveDataHandler.GetUsername(),
                isLeader = true,
                id = 0
            };
            LobbyHandler.AddPlayer(playerSyncData);
        }

        public void ChangeCharacter()
        {
            Session.DeselectCharacter();

            if (NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
            {
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
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