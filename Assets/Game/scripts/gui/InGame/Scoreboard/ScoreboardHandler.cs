using UnityEngine;
using System.Collections.Generic;
using Raider.Game.Networking;
using Raider.Game.Gametypes;

namespace Raider.Game.GUI.Scoreboard
{
    public class ScoreboardHandler : MonoBehaviour
    {
        #region Singleton Setup

        private static List<ScoreboardHandler> instances = new List<ScoreboardHandler>();

        void Awake()
        {
            if (instances == null)
                instances = new List<ScoreboardHandler>();
            instances.Add(this);
        }

        void OnDestroy()
        {
            instances.Remove(this);
        }

        #endregion

        [Header("Active Objects")]
        public GameObject playerContainer;
        public GameObject headerObject;
        [Header("Prefabs Objects")]
        public GameObject playerPlatePrefab;
        public GameObject teamPlatePrefab;

        private void Start()
        {
            InvalidateScoreboard();
        }

        public static void InvalidateScoreboard(GametypeController.SyncListScoreboardPlayer.Operation operation, int index)
        {
            InvalidateScoreboard();
        }

        public static void InvalidateScoreboard()
        {
            if (instances.Count < 1)
                return;

            foreach (ScoreboardHandler instance in instances)
                instance.UpdateScoreboard();
        }

        public void UpdateScoreboard()
        {
            foreach (Transform child in playerContainer.transform)
                Destroy(child.gameObject);

            if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
            {
                for (int i = 0; i < GametypeController.singleton.teamRanking.Count; i++)
                {
                    GameObject teamPlate = Instantiate(teamPlatePrefab);
                    teamPlate.GetComponent<ScoreboardTeamPlate>().SetupPlate((i + 1).ToString(), GametypeController.singleton.teamRanking[i].First.ToString() + " Team", GametypeController.singleton.teamRanking[i].Second, GametypeHelper.GetTeamColor(GametypeController.singleton.teamRanking[i].First), headerObject);
                    teamPlate.transform.SetParent(playerContainer.transform, false);

                    foreach (GametypeController.ScoreboardPlayer player in GametypeController.singleton.PlayerRanking(GametypeController.singleton.teamRanking[i].First))
                    {
                        bool hasLeft = NetworkGameManager.instance.GetPlayerDataById(player.id) == null;

                        GameObject playerPlate = Instantiate(playerPlatePrefab);
                        playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate("", player.emblem, player.name, player.clan, player.score, hasLeft, GametypeHelper.GetTeamColor(player.team), headerObject);
                        playerPlate.transform.SetParent(playerContainer.transform, false);
                    }
                }
            }
            else
            {
                List<GametypeController.ScoreboardPlayer> playerRanking = GametypeController.singleton.PlayerRanking();

                for (int i = 0; i < playerRanking.Count; i++)
                {
                    if (NetworkGameManager.instance.GetPlayerDataById(playerRanking[i].id) == null)
                        continue;

                    GameObject playerPlate = Instantiate(playerPlatePrefab);
                    playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate((i + 1).ToString(), playerRanking[i].emblem, playerRanking[i].name, playerRanking[i].clan, playerRanking[i].score, false, NetworkGameManager.instance.GetPlayerDataById(playerRanking[i].id).syncData.Character.armourPrimaryColor.Color, headerObject);
                    playerPlate.transform.SetParent(playerContainer.transform, false);
                }
            }
        }
    }
}