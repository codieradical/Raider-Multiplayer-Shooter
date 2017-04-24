using UnityEngine;

namespace Raider.Game.Cameras
{
    public class SpectatorThirdPersonCameraController : ThirdPersonCameraController
    {

        public override void Setup()
        {
            base.Setup();

            parent = CameraModeController.singleton.localPlayerGameObject.transform;
            characterController = CameraModeController.singleton.localPlayerGameObject.GetComponent<CharacterController>();
        }

        void Update()
        {
            RotateCamera();
            LockCamPointZRotation();
            UpdateCameraDistance();
        }
    }
}
