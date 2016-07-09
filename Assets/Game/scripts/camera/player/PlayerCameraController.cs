using UnityEngine;

abstract public class PlayerCameraController : CameraController
{
    public CharacterController characterController;
    public Transform playerTransform;
    
    public void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("localPlayer").transform;
        base.parent = playerTransform;
        characterController = playerTransform.gameObject.GetComponent<CharacterController>();

        base.Start();
    }

    public void RotatePlayer()
    {
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * modeController.firstPersonCamSettings.lookSensitivity;

        //Apply rotation
        characterController.transform.Rotate(_rotation);
    }
}