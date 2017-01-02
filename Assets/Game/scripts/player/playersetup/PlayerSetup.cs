using UnityEngine;
using Raider.Game.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;
using System.Collections;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerResourceReferences))]
    public class PlayerSetup : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            PlayerData playerData = GetComponent<PlayerData>();
            PlayerData.localPlayerData = playerData;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SetupLocalPlayer();

            PlayerData.localPlayerData.character = Session.ActiveCharacter;
            PlayerData.localPlayerData.name = Session.saveDataHandler.GetUsername();
            PlayerData.localPlayerData.isLeader = true;
            PlayerData.localPlayerData.gotData = true;

            PlayerData.localPlayerData.gamePlayerController.UpdatePerspective(Session.saveDataHandler.GetSettings().perspective);
        }

        IEnumerator PauseNewCameraController()
        {
            yield return 0;
            yield return 0;
            if(PlayerData.localPlayerData.paused)
                CameraModeController.singleton.GetCameraController().enabled = false;
        }

        void SetupLocalPlayer()
        {
            CameraModeController.singleton.playerGameObject = gameObject;
            CameraModeController.singleton.SetCameraMode(Session.saveDataHandler.GetSettings().perspective);
        }

        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.CameraParent = null;
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
            CameraModeController.singleton.enabled = true;
        }
    }
}