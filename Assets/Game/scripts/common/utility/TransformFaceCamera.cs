using UnityEngine;
using System.Collections;
using Raider.Game.Cameras;

namespace Raider.Common.Utility
{
    public class TransformFaceCamera : MonoBehaviour
    {

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
