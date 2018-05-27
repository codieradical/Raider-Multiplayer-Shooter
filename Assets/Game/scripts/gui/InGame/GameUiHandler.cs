using Raider.Game.Gametypes;
using Raider.Game.GUI.Screens;
using Raider.Game.GUI.StartMenu;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Scene;
using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.GUI
{

    public class GameUiHandler : MonoBehaviour
    {
        #region Singleton Setup

        public static GameUiHandler instance;

        public void Awake()
        {
            if (instance != null)
                Debug.LogAssertion("It seems that multiple Game UI Handlers are active, breaking the singleton instance.");
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public Object optionsPanePrefab;
        public Transform waypointsCanvas;
        public GameObject waypointPrefab;

        void Start()
        {
            NetworkPlayerController.onClientPlayerHealthDead += OnPlayerHealthChange;
            NetworkPlayerController.onClientPlayerRespawned += OnPlayerRespawned;
        }

        // Update is called once per frame
        void Update()
        {
            if (ChatUiHandler.instance.IsOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ChatUiHandler.instance.CloseChatInput();

                    if (!Scenario.InLobby)
                        PlayerData.localPlayerData.localPlayerController.UnpausePlayer();
                }

                return;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!StartMenuHandler.instance.IsOpen)
                        StartMenuHandler.instance.OpenStartMenu();
                    else
                        StartMenuHandler.instance.CloseStartMenu();

                    return;
                }

                if (Input.GetKeyDown(KeyCode.T))
                {
                    if (!StartMenuHandler.instance.IsOpen)
                    {
                        ChatUiHandler.instance.OpenChatInput();
                        if (!Scenario.InLobby)
                            PlayerData.localPlayerData.localPlayerController.PausePlayer();
                    }

                    return;
                }
            }
        }

        public void OnPlayerRespawned(int idRespawned)
        {
            RebuildWaypoints();
        }

        public void OnPlayerHealthChange(int playerID)
        {
            RebuildWaypoints();
        }

        public void RebuildWaypoints()
        {
            foreach (Waypoint waypoint in FindObjectsOfType<Waypoint>())
            {
                Destroy(waypoint.gameObject);
            }

            foreach (PlayerData player in NetworkGameManager.instance.Players)
            {
                GameObject waypointObject = Instantiate(waypointPrefab, waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

                if (player.syncData.team != PlayerData.localPlayerData.syncData.team)
                    waypoint.SetupWaypoint(Waypoint.WaypointIcon.None, player.transform, player.syncData.username);
                else
                    waypoint.SetupPlayerWaypoint(null, player.syncData.username, player.syncData.Character.emblem);
            }

            foreach (PlayerRagdoll ragdoll in FindObjectsOfType<PlayerRagdoll>())
            {
                GameObject waypointObject = Instantiate(waypointPrefab, waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
                waypoint.SetupWaypoint(Waypoint.WaypointIcon.Dead, ragdoll.transform);
            }

            foreach (GametypeObjective objective in FindObjectsOfType<GametypeObjective>())
            {
                Waypoint.WaypointIcon icon;

                if (objective is FlagCaptureObjective)
                    if (objective.team != PlayerData.localPlayerData.syncData.team)
                        continue;
                    else
                        icon = Waypoint.WaypointIcon.Capture;
                else if (objective is FlagObjective)
                    icon = Waypoint.WaypointIcon.Flag;
                else if (objective is HillObjective)
                    icon = Waypoint.WaypointIcon.Hill;
                else if (objective is OddballObjective)
                    icon = Waypoint.WaypointIcon.Ball;
                else
                    icon = Waypoint.WaypointIcon.Generic;

                GameObject waypointObject = Instantiate(waypointPrefab, waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

                waypoint.SetupWaypoint(icon, objective.transform);
            }
        }
    }
}