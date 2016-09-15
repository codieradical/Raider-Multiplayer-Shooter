using UnityEngine;
using System.Collections;
using System;

namespace Raider.Game.Cameras
{

    public class CameraModeController : MonoBehaviour
    {
        #region Singleton Setup

        public static CameraModeController instance;
        public static CameraController controllerInstance
        {
            get { return instance.GetComponent<CameraController>(); }
        }

        public void Awake()
        {
            if (instance != null)
                Debug.LogAssertion("It seems that multiple Camera Mode Controllers are active, breaking the singleton instance.");
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public GameObject camPoint;

        //how close the camera can be to directly overhead or underfoot.
        public float xAxisBuffer = 27f;

        public FirstPersonCameraSettings firstPersonCamSettings;
        public ThirdPersonCameraSettings thirdPersonCamSettings;

        public enum CameraModes
        {
            FirstPerson = 0,
            ThirdPerson = 1,
            Shoulder = 2,
            FlyCam = 3,
            Static = 4,
            Follow = 5,
            SceneOverview = 6,
            FreeCam = 7
        }

        public CameraModes selectedCameraMode = CameraModes.ThirdPerson;
        private CameraModes activeCamera;

        [System.Serializable]
        public class FirstPersonCameraSettings
        {
            public float lookSensitivity = 3f;
            public bool inverted = false;
            public bool moveWithBody = true;
        }

        [System.Serializable]
        public class ThirdPersonCameraSettings
        {
            public LayerMask transparent;
            public float lookSensetivity = 3f;
            public float minDistance = 5f;
            public float maxDistance = 15f;
            public float distanceMoveSpeed = 3f;
            public float cameraPaddingPercent = 0.3f;
            public bool inverted = false;
        }

        Vector3 cameraStartingPoint;

        // Use this for initialization
        void Start()
        {
            camPoint = this.gameObject;

            activeCamera = selectedCameraMode;

            SwitchCameraMode();
        }

        //The method the player uses to change mode.
        void ChangeCameraMode()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //if(seectedCameraMode == CameraModes.FirstPerson)
                //    seectedCameraMode = CameraModes.ThirdPerson;
                //else
                //    seectedCameraMode = CameraModes.FirstPerson;

                selectedCameraMode++;

                if ((int)selectedCameraMode == Enum.GetNames(typeof(CameraModes)).Length)
                {
                    selectedCameraMode = 0;
                }

            }
        }

        //Activates the camera specified in selectedCameraMode.
        void SwitchCameraMode()
        {
            RemoveCameraController();

            activeCamera = selectedCameraMode;

            //Might be a better way to do this, but it beats the old one.
            switch (selectedCameraMode)
            {
                case CameraModes.FirstPerson:
                    gameObject.AddComponent<FirstPersonCameraController>();
                    break;
                case CameraModes.ThirdPerson:
                    gameObject.AddComponent<ThirdPersonCameraController>();
                    break;
                case CameraModes.Shoulder:
                    gameObject.AddComponent<ShoulderCameraController>();
                    break;
                case CameraModes.SceneOverview:
                    gameObject.AddComponent<SceneOverviewCameraController>();
                    break;
                case CameraModes.FreeCam:
                    gameObject.AddComponent<FreeCameraController>();
                    break;
                case CameraModes.FlyCam:
                    gameObject.AddComponent<FlyCameraController>();
                    break;
                case CameraModes.Static:
                    gameObject.AddComponent<StaticCameraController>();
                    break;
                case CameraModes.Follow:
                    gameObject.AddComponent<FollowCameraController>();
                    break;
            }
        }

        public void ChangeCameraParent(Transform _newParent)
        {
            if (_newParent == null)
                Debug.LogWarning("[CameraModeController] Attempted to switch camera parent to null.");
            gameObject.transform.parent = _newParent;
        }

        public void RemoveCameraParent()
        {
            gameObject.transform.parent = null;
        }

        void RemoveCameraController()
        {
            //Remove script of type CameraController
            Destroy(GetComponent<CameraController>());
        }

        void FixedUpdate()
        {
            ChangeCameraMode();

            if (activeCamera != selectedCameraMode)
            {
                SwitchCameraMode();
            }
        }
    }
}