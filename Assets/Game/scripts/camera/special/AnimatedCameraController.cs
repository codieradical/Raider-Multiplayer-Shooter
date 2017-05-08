using UnityEngine;

namespace Raider.Game.Cameras
{

	public class AnimatedCameraController : CameraController
	{
        //Why are these here?
        //Vector3 cameraPosition;
        //Quaternion cameraRotation;

        Animator animator;
        
		AnimatedCameraController()
		{
			camStartingPos = new Vector3(0, 1.8f, 0);
			pointStartingPos = Vector3.zero;
			pointStartingRot = Vector3.zero;
			camStartingRot = Vector3.zero;
			parent = null;
		}

		public override void Setup()
		{
			animator = gameObject.AddComponent<Animator>();
			animator.runtimeAnimatorController = CameraModeController.singleton.animatorController;
		}

		private void OnDestroy()
		{
			Destroy(animator);
		}
	}
}
