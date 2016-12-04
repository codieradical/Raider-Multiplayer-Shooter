using UnityEngine;
using Raider.Game.Networking;
using UnityEngine.Networking;
using Raider.Game.Cameras;

namespace Raider.Game.Player
{

    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(MovementController))]
    public class Player : NetworkBehaviour
    {
        public bool lockCursor = true;

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
            if(isClient || Networking.NetworkManager.instance.currentNetworkState == Networking.NetworkManager.NetworkState.Offline)
            {
                gameObject.AddComponent<MovementController>();
                gameObject.AddComponent<PlayerAnimationController>();
                CameraModeController.singleton.playerGameObject = gameObject;
                transform.Find("Graphics").GetComponent<PlayerAppearenceController>().UpdatePlayerAppearence(transform.name, Networking.NetworkManager.instance.GetMyLobbyPlayer().character);
            }
        }
    }
}