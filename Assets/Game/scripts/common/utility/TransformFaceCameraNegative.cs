using UnityEngine;
using System.Collections;
using Raider.Game.Cameras;

namespace Raider.Common.Utility
{
    public class TransformFaceCameraNegative : TransformFaceCamera
    {

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
