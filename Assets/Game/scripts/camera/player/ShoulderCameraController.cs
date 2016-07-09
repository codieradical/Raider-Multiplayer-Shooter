using UnityEngine;
using System.Collections;

public class ShoulderCameraController : ThirdPersonCameraController {

    ShoulderCameraController()
    {
        pointStartingPos = new Vector3(0, 2f, 0);
        camStartingPos = new Vector3(0.3f, -0.2f, -1.5f);
    }

    void Start()
    {
        base.Start();
        chosenCamDistance = camStartingPos.z;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        LockCamPointZRotation();
    }

    public void ChangeCameraOffsetBasedOnCollision(Vector3 _CollisionPosition)
    {
        //calculate the local position.
        _CollisionPosition = camPoint.transform.InverseTransformPoint(_CollisionPosition);
        //Apply padding percentage.
        _CollisionPosition *= (1 - modeController.thirdPersonCamSettings.cameraPaddingPercent);

        Vector3 newLocation = _CollisionPosition;
        cam.transform.localPosition = newLocation;
    }

    public override void KeepCameraInsideWalls(Vector3 _castToPos)
    {
        RaycastHit objectHitInfo = new RaycastHit();
        float castDistance = Vector3.Distance(transform.position, _castToPos);

        //This cast checks if anything is inbetween transform.position and _castToPos (the camPoint and the _desiredCam).
        bool hitWall = Physics.Raycast(transform.position, (_castToPos - transform.position).normalized, out objectHitInfo, castDistance, ~modeController.thirdPersonCamSettings.transparent);
        float newCamDistance = (cam.transform.localPosition.z - (objectHitInfo.distance - castDistance)) * (1 - modeController.thirdPersonCamSettings.cameraPaddingPercent);
        if (hitWall)
        {
            ChangeCameraOffsetBasedOnCollision(objectHitInfo.point);
        }
        //If no collision is found, cast with infinate distance, to figure out where the camera can go.
        else
        {
            hitWall = Physics.Raycast(transform.position, (_castToPos - transform.position).normalized, out objectHitInfo, ~modeController.thirdPersonCamSettings.transparent);
            //Update the newCamDistance for the last raycast.
            newCamDistance = -objectHitInfo.distance * (1 - modeController.thirdPersonCamSettings.cameraPaddingPercent);
            if (hitWall)
            {
                //If there's more space than the camera needs, just use the chosen distance. (less than because camera distance is negative.)
                if (newCamDistance <= chosenCamDistance)
                {
                    cam.transform.localPosition = camStartingPos;
                }
                //Prevent the camera going ahead of the player.
                else if (newCamDistance > 0)
                {
                    
                }
                //If there's still not enough space, use what is available.
                else
                {
                    ChangeCameraOffsetBasedOnCollision(objectHitInfo.point);
                }
            }
            //If neither raycast hit anything, there's enough space to use the chosen cam distance.
            else
            {
                cam.transform.localPosition = camStartingPos;
            }
        }
    }
}
