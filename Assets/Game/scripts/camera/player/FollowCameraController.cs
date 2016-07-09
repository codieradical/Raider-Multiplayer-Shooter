using UnityEngine;
using System.Collections;

public class FollowCameraController : ThirdPersonCameraController
{
    bool allowYRotation = true;

    //override position and rotation in construct.
    FollowCameraController()
    {
        camStartingPos = new Vector3(0, 0, -5f);
        pointStartingPos = new Vector3(0, 2f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        LockCamPointZRotation();
        RotatePlayer();
        UpdateCameraDistance();
    }

    public void RotateCamera()
    {
        float _yRot = Input.GetAxisRaw("Mouse X");
        float _xRot = Input.GetAxisRaw("Mouse Y");

        if (_xRot != 0 || _yRot != 0 || walking)
        {
            if (!allowYRotation || walking)
            {
                _yRot = 0;
            }

            if (modeController.thirdPersonCamSettings.inverted)
            {
                _xRot = -_xRot;
            }

            Vector3 _camPointRotate = new Vector3(_xRot, _yRot, 0) * modeController.thirdPersonCamSettings.lookSensetivity;

            _camPointRotate = ApplyXBufferToRotation(camPoint.transform.eulerAngles, _camPointRotate);
            KeepCameraRotationWithinWalls(camPoint.transform.eulerAngles, _camPointRotate);

            //Apply rotation
            camPoint.transform.Rotate(_camPointRotate);
        }
    }

    void RotatePlayer()
    {
        float _yRot = Input.GetAxis("Horizontal");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * modeController.firstPersonCamSettings.lookSensitivity;

        //Apply rotation
        characterController.transform.Rotate(_rotation);
    }
}
