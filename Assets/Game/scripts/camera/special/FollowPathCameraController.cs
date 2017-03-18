using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.Cameras
{
    public class FollowPathCameraController : CameraController
    {
        List<GameObject> pathObjects;
        public int lastPointIndex;
        public int nextPointIndex;
        public float speed = 4.0f;
        private float startTime;
        private float journeyLength;


        // Use this for initialization
        public override void Setup()
        {
            base.Setup();

            pathObjects = new List<GameObject>();
            foreach(Transform child in CameraModeController.singleton.cameraPathGameObject.transform)
            {
                pathObjects.Add(child.gameObject);
            }
            //Remove the camera parent.
            CameraModeController.singleton.CameraParent = null;
            camPoint.transform.rotation = pathObjects[0].transform.localRotation;
            camPoint.transform.position = pathObjects[0].transform.position;
            lastPointIndex = pathObjects.Count - 1;
            nextPointIndex = 0;
            HitPoint();
        }

        // Update is called once per frame
        void Update()
        {
            float distCovered = (Time.time - startTime) * speed;
            if (distCovered > Vector3.Distance(pathObjects[lastPointIndex].transform.position, pathObjects[nextPointIndex].transform.position))
                HitPoint();
            else
            {
                float fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(pathObjects[lastPointIndex].transform.position, pathObjects[nextPointIndex].transform.position, fracJourney);
            }
        }

        void HitPoint()
        {
            if (lastPointIndex == pathObjects.Count - 1)
                lastPointIndex = 0;
            else
                lastPointIndex++;

            if (nextPointIndex == pathObjects.Count - 1)
                nextPointIndex = 0;
            else
                nextPointIndex++;

            startTime = Time.time;
            journeyLength = Vector3.Distance(pathObjects[lastPointIndex].transform.position, pathObjects[nextPointIndex].transform.position);

            camPoint.transform.position = pathObjects[lastPointIndex].transform.position;
            camPoint.transform.rotation = pathObjects[lastPointIndex].transform.rotation;
        }
    }
}