using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;

[RequireComponent(typeof(MenuManager))]
public class MainmenuHandler : MonoBehaviour {

    [SerializeField]
    public CharacterEditorHandler editorHandler = new CharacterEditorHandler();

    [Header("Screens")]
    public GameObject LoginScreen;
    public GameObject ChooseCharacterScreen;
    public GameObject CreateCharacterScreen;
    public GameObject MainMenuScreen;

    [Header("Common Components")]
    public HSLColorPicker colorPicker;
    public LobbyHandler lobbyHandler;
    public CharacterPreviewHandler characterPreviewHandler;

    private MenuManager menuManager;

    public void Login(Text _textComponent)
    {
        string _username = _textComponent.text;

        if (string.IsNullOrEmpty(_username))
        {
            //Display info.
            return;
        }

        Session.Login(_username);
        ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
        menuManager.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
    }

    public void CloseCharacterEditor()
    {
        ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().LoadCharacterPlates();
        menuManager.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
    }

    public void ChooseCharacter(int characterIndex)
    {
        Session.SelectCharacter(characterIndex);

        LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate();
        playerNameplate.username = Session.saveDataHandler.GetUsername();
        playerNameplate.leader = true;
        playerNameplate.character = Session.character;

        lobbyHandler.AddPlayer(playerNameplate);

        menuManager.ShowMenu(MainMenuScreen.GetComponent<Menu>());
    }

    public void CreateCharacter()
    {
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().NewCharacter();
        menuManager.ShowMenu(CreateCharacterScreen.GetComponent<Menu>());
    }

    public void EditCharacter(int slot)
    {
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().EditCharacter(slot);
        menuManager.ShowMenu(CreateCharacterScreen.GetComponent<Menu>());
    }

    public void TestLoader()
    {
        SceneManager.LoadScene("PlayerControllerTest");
    }

    // Use this for initialization
    void Start()
    {
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().colorPicker = colorPicker;
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().characterPreviewHandler = characterPreviewHandler;
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().mainmenuHandler = this;
        menuManager = GetComponent<MenuManager>();

        ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().characterPreviewHandler = characterPreviewHandler;
        ChooseCharacterScreen.GetComponent<CharacterSelectionHandler>().mainmenuHandler = this;
    }
}