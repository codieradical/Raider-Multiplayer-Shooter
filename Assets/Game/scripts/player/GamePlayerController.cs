using UnityEngine;
using System.Collections;
using Raider.Game.Cameras;
using Raider.Game.Saves;

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
            SaveDataStructure.Settings settings = Session.saveDataHandler.GetSettings();
            settings.perspective = newPerspective;
            Session.saveDataHandler.SaveSettings(settings);
            CameraModeController.singleton.SetCameraMode(newPerspective);

            StartCoroutine(PauseNewCameraController());

            if (newPerspective == CameraModeController.CameraModes.FirstPerson)
            {
                //Destroy(PlayerData.localPlayerData.graphicsObject);
                PlayerData.localPlayerData.graphicsObject.SetActive(false); //Don't destroy it, just hide it.
                //Later on, first person will have it's own graphics model. So it won't be destroyed.
                PlayerData.localPlayerData.animator.avatar = null;
            }
            else if (newPerspective != CameraModeController.CameraModes.FirstPerson)
            {
                PlayerData.localPlayerData.graphicsObject.SetActive(true);

                if (PlayerData.localPlayerData.appearenceController == null)
                    PlayerData.localPlayerData.appearenceController = GetComponentInChildren<PlayerAppearenceController>();

                PlayerData.localPlayerData.appearenceController.ReplaceGraphicsModel(PlayerData.localPlayerData);
            }

            PlayerAnimationController.RecreateAnimator(PlayerData.localPlayerData, newPerspective);
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