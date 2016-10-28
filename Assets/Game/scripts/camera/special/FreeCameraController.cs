using UnityEngine;
using System.Collections;

namespace Raider.Game.Cameras
{

    public class FreeCameraController : ThirdPersonCameraController
    {
        FreeCameraController()
        {
            camStartingPos = new Vector3(0, 0, 0);
            pointStartingPos = new Vector3(0, 0, 0);
        }

        new void Start()
        {
            base.Start();
            Raider.Game.GUI.UserFeedback.LogError("FreeCam Enabled, press SLASH to switch between camera and player control.");
        }

        bool controllingPlayer = false;
        bool controllingCamera { get { return !controllingPlayer; } set { controllingPlayer = !value; } }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                controllingPlayer = !controllingPlayer;
                Raider.Game.GUI.UserFeedback.LogError("FreeCam: Switched Controls.");
            }

            if (controllingCamera)
            {
                MoveCamera();
                RotateCamera();
                preventMovement = true;
            }

            if(controllingPlayer)
            {
                RotatePlayer();
                preventMovement = false;
            }

            LockCamZRotation();
            LockCamPointZRotation();
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

        new Vector3 KeepCameraInsideWalls(Vector3 _movement)
        {
            Vector3 desiredCamPointPos = camPoint.transform.position + _movement;

            RaycastHit objectHitInfo;
            bool hitwall = Physics.Linecast(camPoint.transform.position, desiredCamPointPos, out objectHitInfo, ~CameraModeController.instance.thirdPersonCamSettings.transparent);
            if (hitwall)
            {
                _movement = Vector3.zero;
            }

            return _movement;
        }
    }
}