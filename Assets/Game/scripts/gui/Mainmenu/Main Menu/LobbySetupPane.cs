using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raider.Game.Scene;

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
        }

        public void StartGame()
        {
            Scenario.instance.LoadScene(selectedScene, selectedGametype);
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