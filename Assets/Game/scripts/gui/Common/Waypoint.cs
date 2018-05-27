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

    public Text labelText;
    public RectTransform icon;
    public WaypointSprites sprites = new WaypointSprites();

    private bool hideMarker;
    private WaypointIcon type;
    private Transform boundObject;

    private string label;
    UserSaveDataStructure.Emblem emblem;

    public void UpdatePosition()
    {
        Debug.Log(CameraModeController.singleton.cam.GetComponent<Camera>());
        Debug.Log(boundObject);
        Camera camera = CameraModeController.singleton.cam.GetComponent<Camera>();
        Vector3 screenPoint = camera.WorldToScreenPoint(boundObject.position);

        if(screenPoint.z < 0)
        {
            if (screenPoint.x > camera.pixelWidth / 2)
                screenPoint.x = 50;
            else
                screenPoint.x = camera.pixelWidth - 50;
        }

        if (screenPoint.x < 0)
            screenPoint.x = 50;
        if (screenPoint.x > camera.pixelWidth)
            screenPoint.x = camera.pixelWidth - 50;

        (transform as RectTransform).position = screenPoint;
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
        this.emblem = emblem;
        SetupWaypoint(type, position, name);
    }
}
