using Raider.Game.Player;
using UnityEngine;

namespace Raider.Game.Cameras
{

    public class FirstPersonCameraController : PlayerCameraController
    {
        //on construction, assign the local starting position.
        public FirstPersonCameraController()
        {
            pointStartingPos = new Vector3(0, 1.7f, 0);
        }

        // Update is called once per frame
        //Every frame, perform these tasks...
        void Update()
        {
            RotatePlayer();
            RotateCamera();
            LockCamPointZRotation();
            LockCamPointYRotation();
            UpdateFirstPersonViewModel();
        }

        void RotateCamera()
        {
            //Looking up and down, needs to be inverted for some reason...
            float _xRot = -Input.GetAxisRaw("Mouse Y");

            //If the camera is set to inverted mode, invert the rotation.
            if (CameraModeController.singleton.firstPersonCamSettings.inverted)
            {
                _xRot = -_xRot;
            }

            Vector3 _rotation = new Vector3(_xRot, 0f, 0f) * CameraModeController.singleton.firstPersonCamSettings.lookSensitivity;

            _rotation = ApplyXBufferToRotation(cam.transform.eulerAngles, _rotation);

            //Apply rotation
            camPoint.transform.Rotate(_rotation);
        }

        //The viewmodel needs to be positioned at the camera, but it needs to be a child of the player.
        //So it's world position is set to the camera's world position on update.
        void UpdateFirstPersonViewModel()
        {
            //The transform of the viewmodel
            Transform viewModel = characterController.gameObject.transform.GetComponent<PlayerData>().firstPersonPlayerModel.transform;
            if (viewModel != null) //If a viewmodel was found...
            {
                //Position and rotate it to match the camera.
                viewModel.rotation = cam.transform.rotation;
                viewModel.position = cam.transform.position;
            }
        }
    }
}