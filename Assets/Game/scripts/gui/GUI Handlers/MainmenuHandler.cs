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
    public GameObject ColorPicker;

    [Header("Lobby")]
    public GameObject Lobby;

    private LobbyHandler lobbyHandler;
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
        menuManager.ShowMenu(ChooseCharacterScreen.GetComponent<Menu>());
    }

    public void ChooseCharacter(int characterIndex)
    {
        //Session.SelectCharacter(characterIndex);

        LobbyHandler.PlayerNameplate playerNameplate = new LobbyHandler.PlayerNameplate();
        //playerNameplate.level = Session.character.level;
        playerNameplate.username = Session.saveDataHandler.GetUsername();
        playerNameplate.leader = true;

        lobbyHandler.AddPlayer(playerNameplate);

        menuManager.ShowMenu(MainMenuScreen.GetComponent<Menu>());
    }

    public void CreateCharacter()
    {
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().NewCharacter();
        menuManager.ShowMenu(GetComponent<Menu>());
    }

    public void TestLoader()
    {
        SceneManager.LoadScene("PlayerControllerTest");
    }

    // Use this for initialization
    void Start()
    {
        CreateCharacterScreen.GetComponent<CharacterEditorHandler>().colorPicker = ColorPicker;
        menuManager = GetComponent<MenuManager>();
        lobbyHandler = Lobby.GetComponent<LobbyHandler>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}