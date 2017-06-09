using UnityEngine;

namespace Raider.Game.Cameras
{
	/// <summary>
	/// This camera controller is ficed to a scene overview object contained within the scene. If no object is found, it will fix to a default location.
	/// </summary>
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