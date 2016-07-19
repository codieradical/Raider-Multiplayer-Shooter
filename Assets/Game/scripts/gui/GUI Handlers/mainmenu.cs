using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class mainmenu : MonoBehaviour {

    public GameObject LoginScreen;
    public GameObject ChooseCharacterScreen;
    public GameObject MainMenuScreen;

    public void Login()
    {

        string _username = LoginScreen.transform.FindChild("usernameInput").FindChild("Text").GetComponent<Text>().text;
        Session.Login(_username);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
