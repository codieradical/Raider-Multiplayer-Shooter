using UnityEngine;
using System.Collections;
using Raider.Game.GUI.Screens;
using Raider.Game.GUI.StartMenu;
using Raider.Game.Scene;
using Raider.Game.Player;

namespace Raider.Game.GUI {

    public class GameUiHandler : MonoBehaviour
    {
        #region Singleton Setup

        public static GameUiHandler instance;

        public void Awake()
        {
            if (instance != null)
                Debug.LogAssertion("It seems that multiple Game UI Handlers are active, breaking the singleton instance.");
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public Object optionsPanePrefab;

        // Update is called once per frame
        void Update()
        {
            if (ChatUiHandler.instance.IsOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ChatUiHandler.instance.CloseChatInput();

                    if (!Scenario.InLobby)
                        PlayerData.localPlayerData.gamePlayerController.UnpausePlayer();
                }

                return;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!StartMenuHandler.instance.IsOpen)
                        StartMenuHandler.instance.OpenStartMenu();
                    else
                        StartMenuHandler.instance.CloseStartMenu();

                    return;
                }

                if(Input.GetKeyDown(KeyCode.T))
                {
                    if (!StartMenuHandler.instance.IsOpen)
                    {
                        ChatUiHandler.instance.OpenChatInput();
                        if (!Scenario.InLobby)
                            PlayerData.localPlayerData.gamePlayerController.PausePlayer();
                    }

                    return;
                }
            }
        }
    }
}