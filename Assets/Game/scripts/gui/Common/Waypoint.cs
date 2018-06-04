using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raider.Game.Saves.User;
using System;
using Raider.Game.Cameras;
using Raider.Game.GUI;

public class Waypoint : MonoBehaviour {

    [Serializable]
    public class WaypointSprites
    {
        public EmblemHandler emblem;
        public RectTransform ball;
        public RectTransform flag;
        public RectTransform hill;
        public RectTransform capture;
        public RectTransform generic;
        public RectTransform dead;
        public RectTransform speaking;
    }

    public enum WaypointIcon
    {
        None,
        Generic,
        Player,
        Speaker,
        Dead,
        Ball,
        Flag,
        Hill,
        Bomb,
        Capture
    }

    public enum WaypointPosition
    {
        Static,
        FollowTransform
    }

    public Text labelText;
    public RectTransform icon;
    public WaypointSprites sprites = new WaypointSprites();

    private bool hideMarker;
    private WaypointIcon type;
    private WaypointPosition positionMode;
    private Transform boundObject;
    private Vector3 worldPosition;

    private string label { get { return labelText.text; } set { labelText.text = value; } }
    UserSaveDataStructure.Emblem emblem;

    public void UpdatePosition()
    {
        Camera camera = CameraModeController.singleton.cam.GetComponent<Camera>();

        //If the bound transform disappears, use it's last known position and switch to static.
        //This is useful for items that despawn like ragdolls.
        if (positionMode == WaypointPosition.FollowTransform)
            if (boundObject == null)
                if (type == WaypointIcon.Dead)
                    Destroy(gameObject);
                else
                    positionMode = WaypointPosition.Static;
            else
                worldPosition = boundObject.position;


        if (type == WaypointIcon.Flag)
        {
            label = ((int)Vector3.Distance(camera.transform.position, worldPosition)).ToString() + "m";
        }

        transform.localScale = Vector3.one * (1 - (Mathf.Clamp((Vector3.Distance(camera.transform.position, worldPosition) / 10), 0, 6) / 10));

        Vector3 screenPoint = camera.WorldToViewportPoint(worldPosition);
        screenPoint.Scale(new Vector3(camera.pixelWidth, camera.pixelHeight, 1));

        if (screenPoint.x < camera.scaledPixelWidth - 50 && screenPoint.x > 50
            && screenPoint.y < camera.scaledPixelHeight - 50 && screenPoint.y > 50
            && screenPoint.z >= 0)
            (transform as RectTransform).position = screenPoint;
        else
        {
            Vector2 origin = new Vector2(camera.scaledPixelWidth / 2, camera.scaledPixelHeight / 2);
            Vector2 marker = new Vector2(screenPoint.x, screenPoint.y);
            if (screenPoint.z < 0)
                marker = new Vector2(camera.scaledPixelWidth - marker.x, camera.scaledPixelHeight - marker.y);

            Vector2 vector = marker - origin;
            vector.Normalize();

            float angle = Mathf.Atan2(vector.y, vector.x);

            float x = Mathf.Clamp((float)Math.Cos(angle) * camera.scaledPixelWidth + camera.scaledPixelWidth / 2, 50, camera.scaledPixelWidth - 50);
            float y = Mathf.Clamp((float)Math.Sin(angle) * camera.scaledPixelHeight + camera.scaledPixelHeight / 2, 50, camera.scaledPixelHeight - 50);

            (transform as RectTransform).position = new Vector3(x, y, 0);
        }
    }

    public void Update()
    {
        UpdatePosition();
    }

    public void SetupWaypoint(WaypointIcon type, Transform position)
    {
        SetupWaypoint(type, position, null);
    }

    public void SetupWaypoint(WaypointIcon type, Transform position, string label)
    {
        if (label == null)
            this.label = "";
        else
            this.label = label;

        this.type = type;
        this.positionMode = WaypointPosition.FollowTransform;
        this.boundObject = position;

        labelText.text = label;

        switch (type)
        {
            case WaypointIcon.Ball:
                sprites.ball.gameObject.SetActive(true);
                break;
            case WaypointIcon.Capture:
                sprites.capture.gameObject.SetActive(true);
                break;
            case WaypointIcon.Flag:
                sprites.flag.gameObject.SetActive(true);
                break;
            case WaypointIcon.Generic:
                sprites.generic.gameObject.SetActive(true);
                break;
            case WaypointIcon.Hill:
                sprites.hill.gameObject.SetActive(true);
                break;
            case WaypointIcon.Player:
                sprites.emblem.gameObject.SetActive(true);
                sprites.emblem.UpdateEmblem(emblem);
                break;
            case WaypointIcon.Dead:
                sprites.dead.gameObject.SetActive(true);
                break;
            case WaypointIcon.Speaker:
                sprites.speaking.gameObject.SetActive(true);
                break;
            case WaypointIcon.None:
                icon.gameObject.SetActive(false);
                break;
        }
    }

    public void SetupPlayerWaypoint(Transform position, string name, UserSaveDataStructure.Emblem emblem)
    {
        this.type = WaypointIcon.Player;
        this.emblem = emblem;
        SetupWaypoint(type, position, name);
    }
}
