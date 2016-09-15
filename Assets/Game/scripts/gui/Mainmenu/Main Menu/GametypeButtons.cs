using UnityEngine;
using System;
using Raider.Game.Scene;

namespace Raider.Game.GUI.Components
{

    public class GametypeButtons : MonoBehaviour
    {

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
        }

        public void HideButtons()
        {
            animatorInstance.SetBool("open", false);
        }

        public void SelectGametype(string gametype)
        {
            HideButtons();
            LobbySetupPane.instance.OpenPane((Scenario.Gametype)Enum.Parse(typeof(Scenario.Gametype), gametype));
        }
    }
}