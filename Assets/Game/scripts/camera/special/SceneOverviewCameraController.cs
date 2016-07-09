using UnityEngine;
using System.Collections;

public class SceneOverviewCameraController : CameraController {

    public Transform parentTransform;

    // Use this for initialization
    void Start () {
        parentTransform = GameObject.Find("_SceneOverview").transform;
        base.parent = parentTransform;
        base.preventMovement = true;

        base.Start();
    }
}
