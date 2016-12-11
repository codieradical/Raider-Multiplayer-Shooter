using UnityEngine;
using Raider.Game.Networking;
using UnityEngine.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;

namespace Raider.Game.Player
{
    public class Player : NetworkBehaviour
    {
        public bool lockCursor = true;
        public SaveDataStructure.Character character;

        public static Player localPlayer;

        // Use this for initialization
        void Start()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            transform.Find("Graphics").GetComponent<PlayerAppearenceController>().UpdatePlayerAppearence(transform.name, character);

            //If the player is a client, or is playing alone, add the moving mechanics.
            if (isLocalPlayer || Networking.NetworkManager.instance.CurrentNetworkState == Networking.NetworkManager.NetworkState.Offline)
            {
                localPlayer = this;
                gameObject.AddComponent<MovementController>();
                gameObject.AddComponent<PlayerAnimationController>();
                CameraModeController.singleton.playerGameObject = gameObject;
                CameraModeController.singleton.SetCameraMode(CameraModeController.CameraModes.ThirdPerson);
            }
        }

        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.CameraParent = null;
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
            CameraModeController.singleton.enabled = true;
        }

        public void PausePlayer()
        {
            GetComponent<MovementController>().enabled = false;
            CameraModeController.singleton.GetCameraController().enabled = false;
            Cursor.visible = true;
        }

        public void UnpausePlayer()
        {
            GetComponent<MovementController>().enabled = true;
            CameraModeController.singleton.GetCameraController().enabled = true;
            Cursor.visible = false;
        }
    }
}