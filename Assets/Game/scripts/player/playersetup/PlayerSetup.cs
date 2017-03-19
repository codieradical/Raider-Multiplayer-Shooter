using Raider.Game.Cameras;
using System.Collections;
using UnityEngine;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(AnimationParametersController))]
    [RequireComponent(typeof(PlayerResourceReferences))]
    public class PlayerSetup : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            PlayerData playerData = GetComponent<PlayerData>();
            PlayerData.localPlayerData = playerData;

            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SetupLocalPlayer();

            PlayerData.localPlayerData.syncData.Character = Session.ActiveCharacter;
            PlayerData.localPlayerData.syncData.username = Session.userSaveDataHandler.GetUsername();
            PlayerData.localPlayerData.syncData.isLeader = true;

            PlayerData.localPlayerData.gamePlayerController.UpdatePerspective(Session.userSaveDataHandler.GetSettings().Perspective);
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
            CameraModeController.singleton.SetCameraMode(Session.userSaveDataHandler.GetSettings().Perspective);
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