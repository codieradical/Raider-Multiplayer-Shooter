using Raider.Game.Cameras;
using UnityEngine;

namespace Raider.Common.Utility
{
	/// <summary>
	/// Makes the object look in the opposite direction of the camera.
	/// </summary>
	public class TransformFaceCameraNegative : TransformFaceCamera
    {
		/// <summary>
		/// Update every frame.
		/// </summary>
        public override void Update()
        {
            LookAtCamera();
        }

        public override void LookAtCamera()
        {
            transform.rotation = Quaternion.LookRotation(-(CameraModeController.singleton.cam.transform.position - transform.position));
        }
    }
}
