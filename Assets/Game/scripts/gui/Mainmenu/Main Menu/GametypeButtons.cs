using Raider.Game.GUI.Screens;
using Raider.Game.Networking;
using Raider.Game.Scene;
using System;
using UnityEngine;

namespace Raider.Game.GUI.Components
{
    [RequireComponent(typeof(Animator))]
    public class GametypeButtons : MonoBehaviour
    {

        //public Text currentMissionText;
        //public Text weeklyHopperText;

        public bool IsOpen
        {
            get { return animatorInstance.GetBool("open"); }
            private set { animatorInstance.SetBool("open", value); }
        }

        public static GametypeButtons instance;

        Animator animatorInstance;

        // Use this for initialization
        void Awake()
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
            IsOpen = true;
            //currentMissionText.text = Session.activeCharacter.currentMission;
        }

        public void HideButtons()
        {
            IsOpen = false;
        }

        public void SelectGametype(string gametype)
        {
            try
            {
				Gametypes.GametypeHelper.Gametype enumGametype = (Gametypes.GametypeHelper.Gametype)Enum.Parse(typeof(Gametypes.GametypeHelper.Gametype), gametype);
                HideButtons();
                LobbySetupPane.instance.OpenPane(enumGametype);
            }
            catch (ArgumentException)
            {
                Debug.LogError("Unable to parse gametype attached to this button.");
            }
        }

        public void MatchmakingButtonClick()
        {
            HideButtons();
            NetworkGameManager.instance.NetworkDiscovery.Initialize();
            NetworkGameManager.instance.NetworkDiscovery.StartAsClient();
            MenuManager.instance.ShowMenu(MainmenuController.instance.MatchmakingScreen.GetComponent<Menu>());
        }
    }
}