using UnityEngine;
using System.Collections;

namespace Raider.Game.Cameras
{

    public class SceneOverviewCameraController : CameraController
    {

        public Transform parentTransform;

        // Use this for initialization
        new void Start()
        {
            parentTransform = CameraModeController.singleton.sceneOverviewGameObject.transform;
            base.parent = parentTransform;
            base.preventMovement = true;

            base.Start();
        }
    }
}