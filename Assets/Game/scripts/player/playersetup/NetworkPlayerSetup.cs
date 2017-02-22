using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Raider.Game.Player;
using Raider.Game.Saves;
using Raider;
using Raider.Game.Cameras;
using Raider.Game.Networking;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerChatManager))]
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(PlayerResourceReferences))]
    public class NetworkPlayerSetup : NetworkBehaviour
    {
        public static NetworkPlayerSetup localPlayer;
        private PlayerData playerData;

        // Use this for initialization

        void Start()
        {
            playerData = GetComponent<PlayerData>();

            //If the player is a client, or is playing alone, add the moving mechanics.
            if (isLocalPlayer)
            {
                PlayerData.localPlayerData = playerData;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SetupLocalPlayer();
            }
        }

        void SetupLocalPlayer()
        {
            localPlayer = this;
            gameObject.AddComponent<MovementController>();
            playerData.gamePlayerController = gameObject.AddComponent<GamePlayerController>();
            CameraModeController.singleton.playerGameObject = gameObject;
            //CameraModeController.singleton.SetCameraMode(Session.saveDataHandler.GetSettings().perspective);
            playerData.gamePlayerController.UpdatePerspective(Session.userSaveDataHandler.GetSettings().perspective);
        }
    }
}
