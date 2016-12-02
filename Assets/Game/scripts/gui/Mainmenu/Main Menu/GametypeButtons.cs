using UnityEngine;
using System;
using Raider.Game.Scene;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{

    public class GametypeButtons : MonoBehaviour
    {

        //public Text currentMissionText;
        //public Text weeklyHopperText;

        public static GametypeButtons instance;

        Animator animatorInstance;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
                Debug.LogWarning("More than one GametypeButtons instance");
            instance = this;
            animatorInstance = GetComponent<Animator>();
        }

        void OnDestroy()
        {
            instance = null;
        }

        public void ShowButtons()
        {
            animatorInstance.SetBool("open", true);
            //currentMissionText.text = Session.activeCharacter.currentMission;
        }

        public void HideButtons()
        {
            animatorInstance.SetBool("open", false);
        }

        public void SelectGametype(string gametype)
        {
            try
            {
                Scenario.Gametype enumGametype = (Scenario.Gametype)Enum.Parse(typeof(Scenario.Gametype), gametype);
                HideButtons();
                LobbySetupPane.instance.OpenPane(enumGametype);
            }
            catch (Exception)
            {
                Debug.LogError("Unable to parse gametype attached to this button.");
            }
        }
    }
}