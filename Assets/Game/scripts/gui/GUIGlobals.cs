using Raider.Game.Gametypes;
using Raider.Game.Networking;
using Raider.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.GUI {

    public class GUIGlobals : MonoBehaviour {

        #region Singleton Setup

        public static GUIGlobals globals;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            if (globals != null)
                Debug.LogAssertion("It seems that multiple Armories are active, breaking the singleton instance.");
            globals = this;
        }

        public void OnDestroy()
        {
            globals = null;
        }

        #endregion

        public class WaypointGlobals
        {
            public GameObject waypointPrefab;
        }

        public WaypointGlobals waypointGlobals;

        public void Update()
        {
            RebuildWaypoints();
        }

        public List<Waypoint> waypoints;
        public void RebuildWaypoints()
        {
            waypoints = new List<Waypoint>();
            foreach(PlayerData player in NetworkGameManager.instance.Players)
            {
                GameObject waypointObject = Instantiate(globals.waypointGlobals.waypointPrefab, GameUiHandler.instance.waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

                if(player.syncData.team != PlayerData.localPlayerData.syncData.team)
                    waypoint.SetupWaypoint(Waypoint.WaypointIcon.None, player.transform, player.syncData.username);
                else
                    waypoint.SetupPlayerWaypoint(null, player.syncData.username, player.syncData.Character.emblem);
            }

            foreach(PlayerRagdoll ragdoll in GetComponents<PlayerRagdoll>())
            {
                GameObject waypointObject = Instantiate(globals.waypointGlobals.waypointPrefab, GameUiHandler.instance.waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
                waypoint.SetupWaypoint(Waypoint.WaypointIcon.Dead, ragdoll.transform);
            }

            foreach(GametypeObjective objective in GetComponents<GametypeObjective>())
            {
                GameObject waypointObject = Instantiate(globals.waypointGlobals.waypointPrefab, GameUiHandler.instance.waypointsCanvas);
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
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

                waypoint.SetupWaypoint(icon, objective.transform);
            }
        }
    }
}
