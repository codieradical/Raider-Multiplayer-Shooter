using Raider.Game.Cameras;
using Raider.Game.Saves.User;
using System.Collections;
using UnityEngine;

namespace Raider.Game.Player
{
    //Unity networking already uses PlayerController
    //And this class is specifically for players in game.
    public class LocalPlayerController : MonoBehaviour
    {
        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.CameraParent = null;
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
            CameraModeController.singleton.enabled = true;
        }

        public void UpdatePerspective(CameraModeController.CameraModes newPerspective)
        {
            UserSaveDataStructure.UserSettings settings = Session.userSaveDataHandler.GetSettings();
            settings.Perspective = newPerspective;
            Session.userSaveDataHandler.SaveSettings(settings, null, null);
            CameraModeController.singleton.SetCameraMode(newPerspective);

            StartCoroutine(PauseNewCameraController());

            //Replace the player model to suit the new perspective.
            PlayerData.localPlayerData.appearenceController.ChangePerspectiveModel(newPerspective);
        }

        public void PausePlayer()
        {
            PlayerData.localPlayerData.paused = true;

            GetComponent<MovementController>().enabled = false;
            PlayerData.localPlayerData.animationController.StopAnimations();
            PlayerData.localPlayerData.animationController.enabled = false;
            CameraModeController.singleton.GetCameraController().enabled = false;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void UnpausePlayer()
        {
            PlayerData.localPlayerData.paused = false;

            GetComponent<MovementController>().enabled = true;
            PlayerData.localPlayerData.animationController.enabled = true;
            CameraModeController.singleton.GetCameraController().enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        IEnumerator PauseNewCameraController()
        {
            yield return 0;
            yield return 0;
            if (PlayerData.localPlayerData.paused)
                CameraModeController.singleton.GetCameraController().enabled = false;
        }
    }
}