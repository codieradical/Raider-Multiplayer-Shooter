using UnityEngine;
using System.Collections;

public class TopDownCameraController : ThirdPersonCameraController
{
    bool turnCamera = false;

    TopDownCameraController()
    {
        camStartingPos = new Vector3(0, 0, -5f);
        pointStartingRot = new Vector3(90f, 0, 0);
    }

    void Update()
    {
        RotatePlayer();

        if (!turnCamera)
            UpdateCameraRotation();

        UpdateCameraDistance();
    }

    void UpdateCameraRotation()
    {
        camPoint.transform.eulerAngles = base.pointStartingRot;
    }
}
