using UnityEngine;
using Raider.Game.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;
using System.Collections;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(Animator))]
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

            PlayerData.localPlayerData.character = Session.activeCharacter;
            PlayerData.localPlayerData.name = Session.saveDataHandler.GetUsername();
            PlayerData.localPlayerData.isLeader = true;
            PlayerData.localPlayerData.gotData = true;

            UpdatePerspective(Session.saveDataHandler.GetSettings().perspective);
        }

        public void UpdatePerspective(CameraModeController.CameraModes newPerspective)
        {
            SaveDataStructure.Settings settings = Session.saveDataHandler.GetSettings();
            settings.perspective = newPerspective;
            Session.saveDataHandler.SaveSettings(settings);
            CameraModeController.singleton.SetCameraMode(newPerspective);

            StartCoroutine(PauseNewCameraController());

            if (newPerspective == CameraModeController.CameraModes.FirstPerson && PlayerData.localPlayerData.graphicsObject != null)
            {
                Destroy(PlayerData.localPlayerData.graphicsObject);
                PlayerData.localPlayerData.animator.avatar = null;
            }
            else if (newPerspective != CameraModeController.CameraModes.FirstPerson && PlayerData.localPlayerData.graphicsObject == null)
                PlayerAppearenceController.SetupGraphicsModel(PlayerData.localPlayerData);
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