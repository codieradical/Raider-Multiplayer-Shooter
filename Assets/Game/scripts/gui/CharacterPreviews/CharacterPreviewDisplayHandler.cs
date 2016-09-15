using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Raider.Game.GUI.CharacterPreviews
{
    [RequireComponent(typeof(EventTrigger))]
    public class CharacterPreviewDisplayHandler : MonoBehaviour
    {

        public bool allowRotation = true;
        public bool allowZoom = true;

        [Header("Zoom Settings")]
        public float minDistance = 1;
        public float maxDistance = 3;

        float cameraDistance
        {
            get { return previewCamera.transform.localPosition.z; }

            set
            {
                Vector3 newPosition;
                newPosition = new Vector3(previewCamera.transform.localPosition.x, previewCamera.transform.localPosition.y, value);
                previewCamera.transform.localPosition = newPosition;
            }
        }

        [HideInInspector] //Assigned by CharacterPreviewHandler(?)
        public GameObject previewCharacterGraphics;
        [HideInInspector]
        public Camera previewCamera;

        float lastMouseX = -1;

        void Start()
        {
            EventTrigger eventTrigger = GetComponent<EventTrigger>();

            if (allowRotation)
            {
                //Create the begin drag callback.
                EventTrigger.Entry begindragEntry = new EventTrigger.Entry();
                begindragEntry.eventID = EventTriggerType.BeginDrag;
                begindragEntry.callback.AddListener((eventData) => { UpdateLastX(eventData); });
                eventTrigger.triggers.Add(begindragEntry);

                //Create the drag callback.
                EventTrigger.Entry dragEntry = new EventTrigger.Entry();
                dragEntry.eventID = EventTriggerType.Drag;
                dragEntry.callback.AddListener((eventData) => { RotatePreview(eventData); });
                eventTrigger.triggers.Add(dragEntry);
            }

            if (allowZoom)
            {
                //Create the scroll callback.
                EventTrigger.Entry scrollEntry = new EventTrigger.Entry();
                scrollEntry.eventID = EventTriggerType.Scroll;
                scrollEntry.callback.AddListener((eventData) => { ZoomPreview(eventData); });
                eventTrigger.triggers.Add(scrollEntry);
            }
        }

        public void RotatePreview(BaseEventData eventData)
        {
            previewCharacterGraphics.transform.Rotate(Vector3.down * (Input.mousePosition.x - lastMouseX));
            UpdateLastX(eventData);
        }

        public void UpdateLastX(BaseEventData eventData)
        {
            lastMouseX = Input.mousePosition.x;
        }

        public void ZoomPreview(BaseEventData eventData)
        {
            float _movement = -Input.GetAxis("Mouse ScrollWheel");

            if ((cameraDistance += _movement) > maxDistance)
                cameraDistance = maxDistance;

            else if ((cameraDistance += _movement) < minDistance)
                cameraDistance = minDistance;

            else
                cameraDistance += _movement;
        }
    }
}