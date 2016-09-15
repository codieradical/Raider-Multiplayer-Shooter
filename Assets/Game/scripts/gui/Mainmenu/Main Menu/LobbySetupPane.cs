using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbySetupPane : MonoBehaviour {

    public static LobbySetupPane instance;

    Animator animatorInstance;

    public Text paneTitle;
    public Text mapLabel;
    public Text networkLabel;
    public Text gametypeLabel;
    public Image mapImage;

    private Scenario.Gametype _selectedGametype;
    public Scenario.Gametype selectedGametype
    {
        get { return _selectedGametype; }
        set
        {
            _selectedGametype = value;
            paneTitle.text = value.ToString();
        }
    }

    private string _selectedScene;
    public string selectedScene
    {
        get { return _selectedScene; }
        set
        {
            mapLabel.text = value;
            _selectedScene = value;
            
            ///mapImage.mainTexture = value;
        }
    }


    public void OpenPane(Scenario.Gametype gametype)
    {
        animatorInstance.SetBool("open", true);
        selectedScene = Scenario.instance.getScenesByGametype(gametype)[0];
    }

    public void ClosePane()
    {
        animatorInstance.SetBool("open", false);
        GametypeButtons.instance.ShowButtons();
    }

    public void StartGame()
    {
        Scenario.instance.LoadScene(selectedScene, selectedGametype);
    }



	// Use this for initialization
	void Start () {
        if (instance != null)
            Debug.LogWarning("Multiple LobbySetupPanes instanced");
        instance = this;

        //This could use tidying.
        if (paneTitle == null || mapLabel == null || networkLabel == null || gametypeLabel == null || mapImage == null)
            Debug.LogWarning("The lobby setup pane is missing some important objects!");

        animatorInstance = GetComponent<Animator>();
        //ClosePane(); //make sure it's closed.
	}

    void OnDestroy()
    {
        instance = null;
    }
}
