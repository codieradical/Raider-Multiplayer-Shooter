using UnityEngine;
using System.Collections;

namespace Raider.Game.Cameras
{

    public class FlyCameraController : ThirdPersonCameraController
    {
        FlyCameraController()
        {
            camStartingPos = new Vector3(0, 2, 0);
            pointStartingPos = new Vector3(0, 0, 0);
        }

        new void Start()
        {
            base.Start();

            base.preventMovement = true;
            base.overrideWalking = false;
        }

        // Update is called once per frame
        void Update()
        {
            RotateCamera();
            MoveCamera();
            LockCamZRotation();
            LockCamPointZRotation();
        }

        new void RotateCamera()
        {
            //Looking up and down, needs to be inverted for some reason...
            float _yRot = Input.GetAxisRaw("Mouse X");
            float _xRot = -Input.GetAxisRaw("Mouse Y");

            //If the camera is set to inverted mode, invert the rotation.
            if (CameraModeController.instance.firstPersonCamSettings.inverted)
            {
                _xRot = -_xRot;
            }

            Vector3 _camPointRotation = new Vector3(_xRot, _yRot, 0f) * CameraModeController.instance.firstPersonCamSettings.lookSensitivity;

            _camPointRotation = ApplyXBufferToRotation(cam.transform.eulerAngles, _camPointRotation);

            //Apply rotation
            camPoint.transform.Rotate(_camPointRotation);
        }

        void MoveCamera()
        {
            float _movX = Input.GetAxis("Horizontal");
            float _movZ = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(_movX, 0, _movZ);
            //Slow it down a little.
            movement *= 0.5f;

            camPoint.transform.Translate(movement);
        }
    }
}