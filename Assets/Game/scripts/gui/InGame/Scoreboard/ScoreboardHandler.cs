using UnityEngine;
using System.Collections.Generic;
using Raider.Game.Networking;
using Raider.Game.Gametypes;
using Raider.Game.Player;
using UnityEngine.UI;

namespace Raider.Game.GUI.Scoreboard
{
	public class ScoreboardHandler : MonoBehaviour
	{
		Animator animatorInstance;
		#region Singleton Setup

		private static List<ScoreboardHandler> instances = new List<ScoreboardHandler>();

		void Awake()
		{
			animatorInstance = GetComponent<Animator>();

			if (instances == null)
				instances = new List<ScoreboardHandler>();

			instances.Add(this);

			if(GametypeController.singleton != null && GametypeController.singleton.hasInitialSpawned)
				headerMessage.text = NetworkGameManager.instance.lobbySetup.syncData.GametypeString;
			else
			{
				headerMessage.text = "Waiting to spawn...";
				Focused = true;
			}

		}

		void OnDestroy()
		{
			instances.Remove(this);
		}

		#endregion

		[Header("Active Objects")]
		public GameObject playerContainer;
		public GameObject headerObject;
		public Text headerMessage;
		[Header("Prefabs Objects")]
		public GameObject playerPlatePrefab;
		public GameObject teamPlatePrefab;

		public bool IsOpen
		{
			get { return animatorInstance.GetBool("open"); }
			set { animatorInstance.SetBool("open", value); }
		}

		public bool Focused
		{
			get { return animatorInstance.GetBool("focused"); }
			set { animatorInstance.SetBool("focused", value); }
		}

		public static bool Open
		{
			get
			{
				return instances[0].IsOpen;
			}
			set
			{
				foreach (ScoreboardHandler instance in instances)
				{
					instance.IsOpen = value;
				}
			}
		}

		public static bool Focus
		{
			get
			{
				return instances[0].Focused;
			}
			set
			{
				foreach (ScoreboardHandler instance in instances)
				{
					instance.Focused = value;
				}
			}
		}

		public static void UpdateHeaderMessage(string message)
		{
			foreach(ScoreboardHandler instance in instances)
			{
				instance.headerMessage.text = message;
			}
		}

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
                for (int i = 0; i < GametypeController.singleton.TeamRanking.Count; i++)
                {
                    GameObject teamPlate = Instantiate(teamPlatePrefab);
                    teamPlate.GetComponent<ScoreboardTeamPlate>().SetupPlate((i + 1).ToString(), GametypeController.singleton.TeamRanking[i].First.ToString() + " Team", GametypeController.singleton.TeamRanking[i].Second, GametypeHelper.GetTeamColor(GametypeController.singleton.TeamRanking[i].First), headerObject);
                    teamPlate.transform.SetParent(playerContainer.transform, false);

                    foreach (GametypeController.ScoreboardPlayer player in GametypeController.singleton.PlayerRanking(GametypeController.singleton.TeamRanking[i].First))
                    {
						PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(player.id);

						bool hasLeft = playerData == null;
						bool isLeader = false;
						bool isDead = false;

						if(!hasLeft)
						{
							isLeader = playerData.PlayerSyncData.isLeader;
							isDead = !playerData.networkPlayerController.IsAlive;
						}

                        GameObject playerPlate = Instantiate(playerPlatePrefab);
                        playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate("", player.emblem, player.name, player.clan, player.score, hasLeft, GametypeHelper.GetTeamColor(player.team), headerObject, isLeader, isDead);
                        playerPlate.transform.SetParent(playerContainer.transform, false);
                    }
                }
            }
            else
            {
				if (GametypeController.singleton == null)
					return;

                List<GametypeController.ScoreboardPlayer> playerRanking = GametypeController.singleton.PlayerRanking();

                for (int i = 0; i < playerRanking.Count; i++)
                {
					PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerRanking[i].id);

					bool hasLeft = playerData == null;
					bool isLeader = false;
					bool isDead = false;

					if (!hasLeft)
					{
						isLeader = playerData.PlayerSyncData.isLeader;
						isDead = !playerData.networkPlayerController.IsAlive;
					}
					else
						continue;

					GameObject playerPlate = Instantiate(playerPlatePrefab);
                    playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate((i + 1).ToString(), playerRanking[i].emblem, playerRanking[i].name, playerRanking[i].clan, playerRanking[i].score, false, NetworkGameManager.instance.GetPlayerDataById(playerRanking[i].id).PlayerSyncData.Character.armourPrimaryColor.Color, headerObject, isLeader, isDead);
                    playerPlate.transform.SetParent(playerContainer.transform, false);
                }
            }
        }
    }
}