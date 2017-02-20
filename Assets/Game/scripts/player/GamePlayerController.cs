using UnityEngine;
using System.Collections;
using Raider.Game.Cameras;
using Raider.Game.Saves;
using Raider.Game.Saves.User;

namespace Raider.Game.Player
{
    //Unity networking already uses PlayerController
    //And this class is specifically for players in game.
    public class GamePlayerController : MonoBehaviour
    {
        void OnDestroy()
        {
            if (GetComponent<PlayerData>().IsLocalPlayer)
            {
                //If the player is being destroyed, save the camera!
                CameraModeController.singleton.CameraParent = null;
                DontDestroyOnLoad(CameraModeController.singleton.camPoint);
                CameraModeController.singleton.enabled = true;
            }
        }

        public void UpdatePerspective(CameraModeController.CameraModes newPerspective)
        {
            UserSaveDataStructure.UserSettings settings = Session.userSaveDataHandler.GetSettings();
            settings.perspective = newPerspective;
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
            GetComponent<PlayerAnimationController>().StopAnimations();
            GetComponent<PlayerAnimationController>().enabled = false;
            CameraModeController.singleton.GetCameraController().enabled = false;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void UnpausePlayer()
        {
            PlayerData.localPlayerData.paused = false;

            GetComponent<MovementController>().enabled = true;
            GetComponent<PlayerAnimationController>().enabled = true;
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