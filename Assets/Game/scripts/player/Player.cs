using UnityEngine;
using Raider.Game.Networking;
using UnityEngine.Networking;
using Raider.Game.Cameras;

namespace Raider.Game.Player
{
    public class Player : NetworkBehaviour
    {
        public bool lockCursor = true;

        public static Player localPlayer;

        // Use this for initialization
        void Start()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            gameObject.name = Session.saveDataHandler.GetUsername();
            //If the player is a client, or is playing alone, add the moving mechanics.
            if(isLocalPlayer || Networking.NetworkManager.instance.currentNetworkState == Networking.NetworkManager.NetworkState.Offline)
            {
                localPlayer = this;

                gameObject.AddComponent<MovementController>();
                gameObject.AddComponent<PlayerAnimationController>();
                CameraModeController.singleton.playerGameObject = gameObject;
                CameraModeController.singleton.SetCameraMode(CameraModeController.CameraModes.ThirdPerson);
                //transform.Find("Graphics").GetComponent<PlayerAppearenceController>().UpdatePlayerAppearence(transform.name, Networking.NetworkManager.instance.GetMyLobbyPlayer().character);
            }
        }

        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.RemoveCameraParent();
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
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