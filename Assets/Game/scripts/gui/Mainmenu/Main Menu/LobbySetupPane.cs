using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raider.Game.Scene;
using System.Collections.Generic;
using Raider.Game.Networking;

namespace Raider.Game.GUI.Components
{

    public class LobbySetupPane : MonoBehaviour
    {

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
                //Replace underscores with spaces.
                paneTitle.text = value.ToString().Replace('_',' ');
            }
        }

        private string _selectedScene;
        public string selectedScene
        {
            get { return _selectedScene; }
            set
            {
                mapLabel.text = value.Replace('_',' ');
                _selectedScene = value;

                mapImage.sprite = GetMapImage(value);
            }
        }


        public void OpenPane(Scenario.Gametype gametype)
        {
            animatorInstance.SetBool("open", true);
            selectedGametype = gametype;
            selectedScene = Scenario.instance.getScenesByGametype(gametype)[0];
        }

        public void ClosePane()
        {
            animatorInstance.SetBool("open", false);
            GametypeButtons.instance.ShowButtons();

            if (NetworkManager.instance.isNetworkActive)
                NetworkManager.instance.StopHost();
        }

        public void StartGame()
        {
            Scenario.instance.LoadScene(selectedScene, selectedGametype);
        }

        public void OpenNetworkOptions()
        {
            List<OptionsPaneOption.OptionsPaneContents> options = new List<OptionsPaneOption.OptionsPaneContents>();

            options.Add(new OptionsPaneOption.OptionsPaneContents("Offline", "Splitscreen co-op. Not Yet Implemented."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Local", "Host a Local Area Nework Lobby. Not Yet Implemented."));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Online", "Host an online lobby on your PC"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Online Server", "Host an online server lobby on your PC"));
            options.Add(new OptionsPaneOption.OptionsPaneContents("Matchmaker/Dedicated", "Not yet implemented."));

            OptionsPaneHandler.instance.ShowOptions("Network", options, SelectNetwork);
        }

        public void SelectNetwork(string option)
        {
            if (option == "Online")
                NetworkManager.instance.StopServer();
                NetworkManager.instance.StartHost();
            if (option == "Online Server")
                NetworkManager.instance.StopHost();
                NetworkManager.instance.StartServer();

        }

        // Use this for initialization
        void Start()
        {
            if (instance != null)
                Debug.LogWarning("Multiple LobbySetupPanes instanced");
            instance = this;

            //This could use tidying.
            if (paneTitle == null || mapLabel == null || networkLabel == null || gametypeLabel == null || mapImage == null)
                Debug.LogWarning("The lobby setup pane is missing some important objects!");

            animatorInstance = GetComponent<Animator>();
        }

        void OnDestroy()
        {
            instance = null;
        }

        Sprite GetMapImage(string mapName)
        {
            //Load singular just wouldn't work!
            Sprite image = Resources.Load<Sprite>("gui/mapImg/" + mapName);
            if (image == null)
            {
                return Resources.Load<Sprite>("gui/mapImg");
            }
            else
                return image;
        }
    }
}