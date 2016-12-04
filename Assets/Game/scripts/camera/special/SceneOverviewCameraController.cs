using UnityEngine;
using System.Collections;

namespace Raider.Game.Cameras
{

    public class SceneOverviewCameraController : CameraController
    {

        public Transform parentTransform;

        // Use this for initialization
        public override void Setup()
        {
            parentTransform = CameraModeController.singleton.sceneOverviewGameObject.transform;
            parent = parentTransform;
            preventMovement = true;

            base.Setup();
        }
    }
}