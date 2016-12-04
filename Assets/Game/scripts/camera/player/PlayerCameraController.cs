using UnityEngine;

namespace Raider.Game.Cameras
{

    abstract public class PlayerCameraController : CameraController
    {
        public CharacterController characterController;

        public override void Setup()
        {
            parent = CameraModeController.singleton.playerGameObject.transform;
            characterController = CameraModeController.singleton.playerGameObject.GetComponent<CharacterController>();

            base.Setup();
        }

        public void RotatePlayer()
        {
            float _yRot = Input.GetAxisRaw("Mouse X");

            Vector3 _rotation = new Vector3(0f, _yRot, 0f) * CameraModeController.singleton.firstPersonCamSettings.lookSensitivity;

            //Apply rotation
            characterController.transform.Rotate(_rotation);
        }
    }
}