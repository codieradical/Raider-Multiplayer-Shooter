using Raider.Game.Cameras;
using UnityEngine;

namespace Raider.Common.Utility
{
	/// <summary>
	/// Simple script to make objects face the camera. Used for player names in world space.
	/// </summary>
	public class TransformFaceCamera : MonoBehaviour
    {
		/// <summary>
		/// Look at the camera every frame.
		/// </summary>
        public virtual void Update()
        {
            LookAtCamera();
        }

        public virtual void LookAtCamera()
        {
            transform.rotation = Quaternion.LookRotation(CameraModeController.singleton.cam.transform.position - transform.position);
        }
    }
}
