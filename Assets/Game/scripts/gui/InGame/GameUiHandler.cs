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
            NetworkPlayerController.onClientPlayerRespawned += OnPlayerAction;
            NetworkPlayerController.onDroppedObjective += OnPlayerAction;
            NetworkPlayerController.onPickedUpObjective += OnPlayerAction;
            NetworkPlayerController.onScoredObjective += OnPlayerAction;

            PlayerRagdoll.onRagdollDespawn += RebuildWaypoints;
            PlayerRagdoll.onRagdollSpawn += RebuildWaypoints;
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

        public void OnPlayerAction(int playerID)
        {
            RebuildWaypoints();
        }

        public void RebuildWaypoints()
        {
            //Not sure why this is needed, so this probably won't be a permanent fix.
            if (waypointPrefab == null || waypointsCanvas == null)
                return;

            foreach (Waypoint waypoint in FindObjectsOfType<Waypoint>())
            {
                Destroy(waypoint.gameObject);
            }

            foreach (PlayerData player in NetworkGameManager.instance.Players)
            {
                if (player == PlayerData.localPlayerData || player.networkPlayerController.Health <= 0)
                    continue;

                GameObject waypointObject = Instantiate(waypointPrefab, waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

                if (player.syncData.team != PlayerData.localPlayerData.syncData.team)
                    waypoint.SetupWaypoint(Waypoint.WaypointIcon.None, player.waypointPosition, player.syncData.username);
                else
                    waypoint.SetupPlayerWaypoint(player.waypointPosition, player.syncData.username, player.syncData.Character.emblem);
            }

            if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
            {
                foreach (PlayerRagdoll ragdoll in FindObjectsOfType<PlayerRagdoll>())
                {
                    if (ragdoll.owner == null || ragdoll.owner.syncData.team != PlayerData.localPlayerData.syncData.team || ragdoll.owner == PlayerData.localPlayerData)
                        continue;

                    GameObject waypointObject = Instantiate(waypointPrefab, waypointsCanvas);
                    Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
                    waypoint.SetupWaypoint(Waypoint.WaypointIcon.Dead, ragdoll.transform);
                }
            }

            foreach (GametypeObjective objective in FindObjectsOfType<GametypeObjective>())
            {
                Waypoint.WaypointIcon icon = Waypoint.WaypointIcon.Generic;

                if (objective is FlagCaptureObjective)
                    if (objective.team != PlayerData.localPlayerData.syncData.team)
                        continue;
                    else
                        icon = Waypoint.WaypointIcon.Capture;
                else if (objective is FlagObjective)
                    if (objective.team == PlayerData.localPlayerData.syncData.team && (objective as FlagObjective).flagOnBase)
                        continue;
                    else if (PlayerData.localPlayerData.networkPlayerController.pickupObjective == objective)
                        continue;
                    else
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