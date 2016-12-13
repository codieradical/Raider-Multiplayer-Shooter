using UnityEngine;
using System.Collections;
using Raider.Game.GUI.Screens;
using Raider.Game.GUI.StartMenu;
using Raider.Game.Scene;

namespace Raider.Game.GUI {

    public class GameUiHandler : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (ChatUiHandler.instance.IsOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ChatUiHandler.instance.IsOpen = false;

                    if (!Scenario.InLobby)
                        Player.Player.localPlayer.UnpausePlayer();
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
                    ChatUiHandler.instance.OpenChatInput();
                    if(!Scenario.InLobby)
                        Player.Player.localPlayer.PausePlayer();

                    return;
                }
            }
        }
    }
}