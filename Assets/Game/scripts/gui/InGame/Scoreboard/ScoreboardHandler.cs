using UnityEngine;
using System.Collections.Generic;
using Raider.Game.Networking;
using Raider.Game.Gametypes;
using Raider.Game.Player;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Raider.Game.GUI.Scoreboard
{
	public class ScoreboardHandler : MonoBehaviour
	{
        public static Action scoreboardHUDInvalidate;

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
			//NetworkPlayerController.onClientPlayerHealthDead += PlayerDied;
			//NetworkPlayerController.onClientPlayerHealthAlive += PlayerRespawned;


            if(GametypeController.singleton != null && GametypeController.singleton.scoreboard != null)
                InvalidateScoreboard();
        }

        public static void InvalidateScoreboard()
        {
            if (instances.Count < 1)
                return;

            foreach (ScoreboardHandler instance in instances)
                instance.UpdateScoreboard();

            if (scoreboardHUDInvalidate != null)
                scoreboardHUDInvalidate();
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

                    foreach (GametypeController.ScoreboardPlayer player in GametypeController.singleton.PlayerRanking(GametypeController.singleton.TeamRanking[i].First).First)
                    {
						PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(player.id);

						bool isLeader = false;
						bool isDead = false;

						isLeader = playerData.PlayerSyncData.isLeader;
						isDead = !playerData.networkPlayerController.IsAlive;

                        GameObject playerPlate = Instantiate(playerPlatePrefab);
                        playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate(player.id, player.team, "", player.emblem, player.name, player.clan, player.score, false, GametypeHelper.GetTeamColor(player.team), headerObject, isLeader, isDead);
                        playerPlate.transform.SetParent(playerContainer.transform, false);
                    }

                    foreach (GametypeController.ScoreboardPlayer player in GametypeController.singleton.PlayerRanking(GametypeController.singleton.TeamRanking[i].First).Second)
                    {
                        bool isLeader = false;
                        bool isDead = false;

                        GameObject playerPlate = Instantiate(playerPlatePrefab);
                        playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate(-1, player.team, "", player.emblem, player.name, player.clan, player.score, true, GametypeHelper.GetTeamColor(player.team), headerObject, isLeader, isDead);
                        playerPlate.transform.SetParent(playerContainer.transform, false);
                    }
                }
            }
            else
            {
				if (GametypeController.singleton == null)
					return;

                List<GametypeController.ScoreboardPlayer> activePlayerRanking = GametypeController.singleton.PlayerRanking().First;

                for (int i = 0; i < activePlayerRanking.Count; i++)
                {
					PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(activePlayerRanking[i].id);

					bool isLeader = false;
					bool isDead = false;

					isLeader = playerData.PlayerSyncData.isLeader;
					isDead = !playerData.networkPlayerController.IsAlive;

					GameObject playerPlate = Instantiate(playerPlatePrefab);
                    playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate(activePlayerRanking[i].id, activePlayerRanking[i].team, (i + 1).ToString(), activePlayerRanking[i].emblem, activePlayerRanking[i].name, activePlayerRanking[i].clan, activePlayerRanking[i].score, false, activePlayerRanking[i].color, headerObject, isLeader, isDead);
                    playerPlate.transform.SetParent(playerContainer.transform, false);
                }
                List<GametypeController.ScoreboardPlayer> inactivePlayerRanking = GametypeController.singleton.PlayerRanking().Second;

                for (int i = 0; i < inactivePlayerRanking.Count; i++)
                {
                    PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(activePlayerRanking[i].id);

                    bool isLeader = false;
                    bool isDead = false;

                    GameObject playerPlate = Instantiate(playerPlatePrefab);
                    playerPlate.GetComponent<ScoreboardPlayerPlate>().SetupPlate(-1, inactivePlayerRanking[i].team, (i + 1 + activePlayerRanking.Count).ToString(), inactivePlayerRanking[i].emblem, inactivePlayerRanking[i].name, inactivePlayerRanking[i].clan, inactivePlayerRanking[i].score, true, inactivePlayerRanking[i].color, headerObject, isLeader, isDead);
                    playerPlate.transform.SetParent(playerContainer.transform, false);
                }
            }
        }

		void Update()
		{
			UpdatePlayerState();
		}

		void UpdatePlayerState()
		{
			try
			{
				foreach (ScoreboardPlayerPlate plate in playerContainer.GetComponentsInChildren<ScoreboardPlayerPlate>())
				{
					NetworkPlayerController networkPlayerController = NetworkGameManager.instance.GetPlayerDataById(plate.playerId).networkPlayerController;
					if (networkPlayerController != null && !networkPlayerController.IsAlive)
						plate.IsDead = true;
					else
						plate.IsDead = false;
				}
			}
			catch (NullReferenceException) { }
		}

		ScoreboardPlayerPlate GetPlayerPlateByIDAndTeam(int playerID, GametypeHelper.Team team)
		{
            //I should probably be using GetComponentsInChildren, but the function is broken, so maybe not.

            foreach(Transform transform in playerContainer.transform)
            {
                if(transform.gameObject.activeSelf)
                {
                    ScoreboardPlayerPlate plate = transform.gameObject.GetComponent<ScoreboardPlayerPlate>();

                    if (plate == null) continue;

                    if (plate.playerId == playerID && plate.playerTeam == team)
                        return plate;
                }
            }

			return null;
		}

		//public void PlayerDied(int killed)
		//{
		//	PlayerData.SyncData killedPlayer = NetworkGameManager.instance.GetPlayerDataById(killed).syncData;

		//	ScoreboardPlayerPlate playerPlate = GetPlayerPlateByIDAndTeam(killedPlayer.id, killedPlayer.team);
		//	if(playerPlate != null)
		//		playerPlate.IsDead = true;
  //          //else
  //          //    StartCoroutine(UpdateDeadPlateState(killedPlayer.id, killedPlayer.team, true));
		//}

  //      //When a player is killed, the score changes, and the scoreboard is invalidated, and during invalidation
  //      //plates are destroyed. Until the next frame, those plates are null.
  //      //Due to the unpredicable nature of network syncing, the easiest solution is to provide an
  //      //additional frame to update. If it didn't work last time, try again.
  //      //private IEnumerator UpdateDeadPlateState(int id, GametypeHelper.Team team, bool dead)
  //      //{
  //      //    //Waiit a couple frames...
  //      //    yield return new WaitForFixedUpdate();
  //      //    //yield return new WaitForFixedUpdate();
  //      //    ScoreboardPlayerPlate playerPlate = GetPlayerPlateByIDAndTeam(id, team);
  //      //    if (playerPlate != null)
  //      //        playerPlate.IsDead = dead;
  //      //    yield return 0;
  //      //}

		//public void PlayerRespawned(int respawned)
		//{
		//	PlayerData.SyncData respawnedPlayer = NetworkGameManager.instance.GetPlayerDataById(respawned).syncData;

		//	ScoreboardPlayerPlate playerPlate = GetPlayerPlateByIDAndTeam(respawnedPlayer.id, respawnedPlayer.team);
		//	if (playerPlate != null)
		//		playerPlate.IsDead = false;
  //          //else
  //          //    StartCoroutine(UpdateDeadPlateState(respawnedPlayer.id, respawnedPlayer.team, false));
  //      }
    }
}