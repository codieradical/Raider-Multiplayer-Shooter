using Raider.Game.GUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Raider.Game.Cameras
{

    public class CameraModeController : MonoBehaviour
    {
        #region Singleton Setup

        public static CameraModeController singleton;
        public static CameraController ControllerInstance
        {
            get { return singleton.GetComponent<CameraController>(); }
        }

        public void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (singleton != null)
                Debug.LogAssertion("It seems that multiple Camera Mode Controllers are active, breaking the singleton instance.");
            singleton = this;
        }

        public void OnDestroy()
        {
            singleton = null;
        }

        #endregion

        public const string SCENE_OVERVIEW_OBJECT_NAME = "_SceneOverview";
        public const string CAMERA_PATH_OBJECT_NAME = "_CameraPath";

        public GameObject camPoint;
        public GameObject cam;
        public GameObject localPlayerGameObject;
        public int spectatingPlayerIndex;

        //how close the camera can be to directly overhead or underfoot.
        public float xAxisBuffer = 35f;

        public FirstPersonCameraSettings firstPersonCamSettings;
        public ThirdPersonCameraSettings thirdPersonCamSettings;

        public GameObject sceneOverviewGameObject;
        public GameObject cameraPathGameObject;
		public RuntimeAnimatorController animatorController;

        [Serializable]
        public class FirstPersonCameraSettings
        {
            public float lookSensitivity = 3f;
            public bool inverted = false;
            public bool moveWithBody = true;
        }

        [Serializable]
        public class ThirdPersonCameraSettings
        {
            public LayerMask transparent;
            public float lookSensitivity = 3f;
            public float minDistance = 5f;
            public float maxDistance = 15f;
            public float distanceMoveSpeed = 3f;
            public float cameraPaddingPercent = 0.3f;
            public bool inverted = true;
        }

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
            FollowPath = 9,
			Animated = 10,
			SpectatorThirdPerson = 11
        }

        public CameraModes CameraMode
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
				else if (attachedController is AnimatedCameraController)
					return CameraModes.Animated;
				else if (attachedController is SpectatorThirdPersonCameraController)
					return CameraModes.SpectatorThirdPerson;

                return CameraModes.Unknown;
            }

            //This can only be set once per frame, so a custom Change Camera method will ensure that happens.
            private set
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
						case CameraModes.Animated:
							newController = gameObject.AddComponent<AnimatedCameraController>();
							break;
						case CameraModes.SpectatorThirdPerson:
							newController = gameObject.AddComponent<SpectatorThirdPersonCameraController>();
							break;
                    }
                }
                if (newController != null)
                    newController.Setup();
            }
        }

        public Queue<CameraModes> cameraModeUpdates = new Queue<CameraModes>();

        public void SetCameraMode(CameraModes newMode)
        {
            cameraModeUpdates.Enqueue(newMode);
        }

        public CameraController GetCameraController()
        {
            if (CameraMode > 0)
                return GetComponent<CameraController>();
            else
                return null;
        }

        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            camPoint = gameObject;
            cam = camPoint.transform.Find("Camera").gameObject;
        }

        //Camera mode changes need to be performed frame by frame, to ensure that controllers have been initialized.
        void Update()
        {
            if(cameraModeUpdates.Count > 0)
                CameraMode = cameraModeUpdates.Dequeue();
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene newScene, LoadSceneMode newSceneLoadMode)
        {
            //If a new single scene has been loaded, setup the camera.
            if (newSceneLoadMode == LoadSceneMode.Single)
            {
                //When the scene changes, we don't need the old updates anymore.
                cameraModeUpdates = new Queue<CameraModes>();

                CameraMode = CameraModes.None;

                SetupSceneCamera();
            }
        }

        void SetupSceneCamera()
        {
            sceneOverviewGameObject = GameObject.Find(SCENE_OVERVIEW_OBJECT_NAME);
            if (sceneOverviewGameObject != null)
                cameraModeUpdates.Enqueue(CameraModes.SceneOverview);
            else
                cameraPathGameObject = GameObject.Find(CAMERA_PATH_OBJECT_NAME);
                if(cameraPathGameObject != null)
                    cameraModeUpdates.Enqueue(CameraModes.FollowPath);
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


        //Debug, throw me into update.
        void ChangeCameraMode()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CameraMode++;

                if ((int)CameraMode == Enum.GetNames(typeof(CameraModes)).Length)
                {
                    CameraMode = 0;
                }

                UserFeedback.LogError("Changed Camera Mode to " + CameraMode.ToString());
            }
        }

        //Usually a set parent method would be called,
        //But each camera controller inherently updates the position on start.
        public Transform CameraParent
        {
            set{ gameObject.transform.SetParent(value, false); }
            get { return camPoint.transform.parent; }
        }

    }
}