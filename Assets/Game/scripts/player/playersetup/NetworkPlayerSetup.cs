using Raider.Game.Cameras;
using UnityEngine;
using UnityEngine.Networking;

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

                gameObject.layer = LayerMask.NameToLayer("LocalPlayer");

				gameObject.name = Session.userSaveDataHandler.GetUsername();

                localPlayer = this;

                playerData.networkPlayerController.SpawnWeapon(playerData.syncData.Character.PrimaryWeapon);
            }

            playerData.appearenceController.ReplacePlayerModel(playerData);

            if(isLocalPlayer)
            {
                playerData.appearenceController.ChangePerspectiveModel(Session.userSaveDataHandler.GetSettings().Perspective);
            }
        }

        [TargetRpc]
        public void TargetSetupLocalControl(NetworkConnection conn)
        {
            gameObject.AddComponent<MovementController>();
            playerData.animationController = gameObject.AddComponent<AnimationParametersController>();
            playerData.gamePlayerController = gameObject.AddComponent<LocalPlayerController>();
            CameraModeController.singleton.playerGameObject = gameObject;
            //CameraModeController.singleton.SetCameraMode(Session.saveDataHandler.GetSettings().perspective);
            playerData.gamePlayerController.UpdatePerspective(Session.userSaveDataHandler.GetSettings().Perspective);
        }

        //Detatch Camera, Prototype.
        private void OnDestroy()
        {
            if (isLocalPlayer)
            {
                if (CameraModeController.singleton.GetCameraController() is PlayerCameraController)
                    CameraModeController.singleton.gameObject.transform.SetParent(null, false);
            }
        }
    }
}
