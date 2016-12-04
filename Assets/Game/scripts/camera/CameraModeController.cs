using UnityEngine;
using System;
using Raider.Game.GUI;
using UnityEngine.SceneManagement;

namespace Raider.Game.Cameras
{

    public class CameraModeController : MonoBehaviour
    {
        #region Singleton Setup

        public static CameraModeController singleton;
        public static CameraController controllerInstance
        {
            get { return singleton.GetComponent<CameraController>(); }
        }

        public void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            if (singleton != null)
                Debug.LogAssertion("It seems that multiple Camera Mode Controllers are active, breaking the singleton instance.");
            singleton = this;
        }

        public void OnDestroy()
        {
            singleton = null;
        }

        #endregion

        public GameObject camPoint;
        public GameObject cam;
        public GameObject playerGameObject;

        //how close the camera can be to directly overhead or underfoot.
        public float xAxisBuffer = 27f;

        public FirstPersonCameraSettings firstPersonCamSettings;
        public ThirdPersonCameraSettings thirdPersonCamSettings;

        public GameObject sceneOverviewGameObject;
        public GameObject cameraPathGameObject;

        public enum CameraModes
        {
            Unknown = -1,
            None = 0,
            FirstPerson = 1,
            ThirdPerson = 2,
            Shoulder = 3,
            FlyCam = 4,
            Static = 5,
            Follow = 6,
            SceneOverview = 7,
            FreeCam = 8,
            FollowPath = 9
        }

        public CameraModes cameraMode
        {
            get
            {
                CameraController attachedController = GetComponent<CameraController>();

                if (attachedController == null)
                    return CameraModes.None;
                else if (attachedController is FirstPersonCameraController)
                    return CameraModes.FirstPerson;
                else if (attachedController is ThirdPersonCameraController)
                    return CameraModes.ThirdPerson;
                else if (attachedController is ShoulderCameraController)
                    return CameraModes.Shoulder;
                else if (attachedController is FlyCameraController)
                    return CameraModes.FlyCam;
                else if (attachedController is StaticCameraController)
                    return CameraModes.Static;
                else if (attachedController is FollowCameraController)
                    return CameraModes.Follow;
                else if (attachedController is SceneOverviewCameraController)
                    return CameraModes.SceneOverview;
                else if (attachedController is FreeCameraController)
                    return CameraModes.FreeCam;
                else if (attachedController is FollowPathCameraController)
                    return CameraModes.FollowPath;

                return CameraModes.Unknown;
            }

            set
            {
                RemoveCameraController();

                //Grab it now, or it'll only be retrievable next frame.
                CameraController newController = null;

                if (value == CameraModes.None)
                {
                    transform.Find("Camera").GetComponent<Camera>().enabled = false;
                }
                else
                {
                    transform.Find("Camera").GetComponent<Camera>().enabled = true;

                    switch (value)
                    {
                        case CameraModes.FirstPerson:
                            newController = gameObject.AddComponent<FirstPersonCameraController>();
                            break;
                        case CameraModes.ThirdPerson:
                            newController = gameObject.AddComponent<ThirdPersonCameraController>();
                            break;
                        case CameraModes.Shoulder:
                            newController = gameObject.AddComponent<ShoulderCameraController>();
                            break;
                        case CameraModes.SceneOverview:
                            newController = gameObject.AddComponent<SceneOverviewCameraController>();
                            break;
                        case CameraModes.FreeCam:
                            newController = gameObject.AddComponent<FreeCameraController>();
                            break;
                        case CameraModes.FlyCam:
                            newController = gameObject.AddComponent<FlyCameraController>();
                            break;
                        case CameraModes.Static:
                            newController = gameObject.AddComponent<StaticCameraController>();
                            break;
                        case CameraModes.Follow:
                            newController = gameObject.AddComponent<FollowCameraController>();
                            break;
                        case CameraModes.FollowPath:
                            newController = gameObject.AddComponent<FollowPathCameraController>();
                            break;
                    }
                }
                if (newController != null)
                    newController.Setup();
            }
        }

        public CameraController GetCameraController()
        {
            if (cameraMode > 0)
                return GetComponent<CameraController>();
            else
                return null;
        }

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
            public float lookSensitivity = 3f;
            public float minDistance = 5f;
            public float maxDistance = 15f;
            public float distanceMoveSpeed = 3f;
            public float cameraPaddingPercent = 0.3f;
            public bool inverted = false;
        }

        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            camPoint = gameObject;
            cam = camPoint.transform.Find("Camera").gameObject;
        }

        void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
        {
            cameraMode = CameraModes.None;

            SetupSceneCamera();
        }

        void SetupSceneCamera()
        {
            sceneOverviewGameObject = GameObject.Find("_SceneOverview");
            if (sceneOverviewGameObject != null)
                cameraMode = CameraModes.SceneOverview;
            else
                cameraPathGameObject = GameObject.Find("_CameraPath");
                if(cameraPathGameObject != null)
                    cameraMode = CameraModes.FollowPath;
        }

        void SetupCameraController()
        {
            if (GetCameraController() == null)
                return;
            else
                GetCameraController().Setup();
        }

        void RemoveCameraController()
        {
            //Remove script of type CameraController
            Destroy(GetComponent<CameraController>());
        }


        //The method the player uses to change mode.
        void ChangeCameraMode()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                cameraMode++;

                if ((int)cameraMode == Enum.GetNames(typeof(CameraModes)).Length)
                {
                    cameraMode = 0;
                }

                UserFeedback.LogError("Changed Camera Mode to " + cameraMode.ToString());
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
    }
}