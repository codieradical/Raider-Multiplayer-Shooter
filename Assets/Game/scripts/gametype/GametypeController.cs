using Raider.Common.Types;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.GUI.Scoreboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Raider.Game.Saves.User;

namespace Raider.Game.Gametypes
{
    public abstract class GametypeController : NetworkBehaviour
    {
		#region singleton setup
		public static GametypeController singleton;

		void Awake()
		{
			if (singleton != null)
				Debug.LogError("More than one Gametype active! What are you doing!!");
			singleton = this;

			ScoreboardHandler.InvalidateScoreboard();
		}

		void OnDestroy()
		{
			singleton = null;
		}

        #endregion

        #region Scoring

        public override void OnStartClient()
        {
            scoreboard.Callback = ScoreboardHandler.InvalidateScoreboard;
        }

        public struct ScoreboardPlayer
		{
			public ScoreboardPlayer(int id, int score, GametypeHelper.Team team, string name, string clan, UserSaveDataStructure.Emblem emblem, bool hasLeft)
			{
				this.id = id;
				this.score = score;
				this.team = team;
                this.name = name;
                this.clan = clan;
                this.emblem = emblem;
				this.hasLeft = hasLeft;
			}
			public int id;
            public string name;
            public string clan;
            public UserSaveDataStructure.Emblem emblem;
			public int score;
			public GametypeHelper.Team team;
			public bool hasLeft;
		}

		public class SyncListScoreboardPlayer : SyncListStruct<ScoreboardPlayer>
		{

		}

		public SyncListScoreboardPlayer scoreboard = new SyncListScoreboardPlayer();

		public void AddPlayerToScoreboard(int playerId)
		{
			PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerId);
			if(playerData == null)
			{
				Debug.Log("Unable to add Player " + playerId + "to scoreboard.");
				return;
			}

			//If the player is already on the scoreboard, just make sure their team is alright.
			foreach(ScoreboardPlayer player in scoreboard)
			{
				if (player.id == playerData.PlayerSyncData.id && player.team == playerData.PlayerSyncData.team)
				{
					return;
				}
			}

			scoreboard.Add(new ScoreboardPlayer(playerData.PlayerSyncData.id, 0, playerData.PlayerSyncData.team, playerData.PlayerSyncData.username, playerData.PlayerSyncData.Character.guild, playerData.PlayerSyncData.Character.emblem, false));
		}

		public void RemovePlayer(int playerId)
		{
			foreach(ScoreboardPlayer player in scoreboard)
			{
				if(player.id == playerId)
				{
					scoreboard.Remove(player);
					break;
				}
			}
		}

		public List<Tuple<GametypeHelper.Team, int>> TeamRanking
		{
			get
			{
				List<Tuple<GametypeHelper.Team, int>> teamScores = new List<Tuple<GametypeHelper.Team, int>>();
				foreach (GametypeHelper.Team team in Enum.GetValues(typeof(GametypeHelper.Team)))
				{
					if (PlayerRanking(team).Count < 1)
					{
						int teamTotal;
						if (GetTeamTotal(team, out teamTotal))
						{
							teamScores.Add(new Tuple<GametypeHelper.Team, int>(team, teamTotal));
							continue;
						}
						else
							continue;
					}

					int iterationScore = 0;

					foreach (ScoreboardPlayer player in scoreboard)
					{
						if (player.team == team)
							iterationScore += player.score;
					}
					teamScores.Add(new Tuple<GametypeHelper.Team, int>(team, iterationScore));
				}
				return (teamScores.OrderBy(team => PlayerRanking(team.First).Count() < 1).ThenByDescending(team => team.Second).ThenBy(team => team.First)).ToList();
			}
		}

		public bool GetTeamTotal(GametypeHelper.Team team, out int total)
		{
			List<ScoreboardPlayer> players = new List<ScoreboardPlayer>();
			total = 0;

			foreach (ScoreboardPlayer player in scoreboard)
			{
				if (player.team == team)
				{
					players.Add(player);
					continue;
				}
			}

			if (players.Count < 1)
				return false;
			else
			{
				foreach (ScoreboardPlayer player in players)
					total += player.score;

				return true;
			}
		}

        public List<ScoreboardPlayer> PlayerRanking(GametypeHelper.Team team)
        {
            List<ScoreboardPlayer> players = new List<ScoreboardPlayer>();

			foreach (ScoreboardPlayer player in scoreboard)
			{
				if (!player.hasLeft && player.team == team)
				{
					players.Add(player);
					continue;
				}

				bool playerInAnotherTeam = false;

				foreach (ScoreboardPlayer player2 in scoreboard)
				{
					if (player.id == player2.id)
					{
						playerInAnotherTeam = true;
						break;
					}
				}

				if (playerInAnotherTeam)
					continue;

				players.Add(player);
			}

            return (players.OrderBy(player => !player.hasLeft).ThenByDescending(player => player.score).ThenBy(player => player.id)).ToList();
        }

        public List<ScoreboardPlayer> PlayerRanking()
        {
            return (scoreboard.OrderBy(player => !player.hasLeft).ThenByDescending(player => player.score).ThenBy(player => player.id)).ToList();
        }

		[Server]
		public void AddToScoreboard(int playerId, GametypeHelper.Team playerTeam, int addition)
		{
			if (isGameEnding)
				return;

			for (int i = 0; i < scoreboard.Count; i++)
			{
				if (scoreboard[i].id == playerId && scoreboard[i].team == playerTeam)
				{
					ScoreboardPlayer updatedScore = new ScoreboardPlayer(scoreboard[i].id, scoreboard[i].score + addition, scoreboard[i].team, scoreboard[i].name, scoreboard[i].clan, scoreboard[i].emblem, scoreboard[i].hasLeft);
					scoreboard[i] = updatedScore;
					return;
				}
			}
			PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerId);
			ScoreboardPlayer newScore = new ScoreboardPlayer(playerId, addition, playerTeam, playerData.PlayerSyncData.username, playerData.PlayerSyncData.Character.guild, playerData.PlayerSyncData.Character.emblem, false);
			scoreboard.Add(newScore);

			if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
			{
				if (TeamRanking[0].Second >= NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin)
					StartCoroutine(GameOver());
			}
			else if (newScore.score >= NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin)
				StartCoroutine(GameOver());
		}

		[Server]
		public void UpdateScoreboardActivePlayers()
		{
			List<Tuple<int, GametypeHelper.Team>> activePlayersAndTeams = new List<Tuple<int, GametypeHelper.Team>>();

			foreach (PlayerData player in NetworkGameManager.instance.Players)
				activePlayersAndTeams.Add(new Tuple<int, GametypeHelper.Team>(player.PlayerSyncData.id, player.PlayerSyncData.team));

			for (int i = 0; i < scoreboard.Count; i++)
			{
				bool foundPlayer = false;
				foreach (Tuple<int, GametypeHelper.Team> activePlayer in activePlayersAndTeams)
				{
					if (scoreboard[i].id == activePlayer.First && scoreboard[i].team == activePlayer.Second)
					{
						scoreboard[i] = new ScoreboardPlayer(scoreboard[i].id, scoreboard[i].score, scoreboard[i].team, scoreboard[i].name, scoreboard[i].clan, scoreboard[i].emblem, false);
						foundPlayer = true;
						break;
					}
				}

				if (foundPlayer)
					continue;

				scoreboard[i] = new ScoreboardPlayer(scoreboard[i].id, scoreboard[i].score, scoreboard[i].team, scoreboard[i].name, scoreboard[i].clan, scoreboard[i].emblem, true);
			}
		}

		public string GetWinner()
		{
			if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
				return TeamRanking[0].First.ToString();
			else
				return PlayerRanking()[0].name;
		}

		#endregion

		[Serializable]
        public class GameOptions
        {
            public int scoreToWin = 50;
            public bool teamsEnabled;
            public GametypeOptions gametypeOptions;
            public TeamOptions teamOptions = new TeamOptions();
            public GeneralOptions generalOptions = new GeneralOptions();

            [Serializable]
            public abstract class GametypeOptions
            {
                public GametypeOptions()
                {

                }
            }

            [Serializable]
            public class TeamOptions
            {
                public int maxTeams = Enum.GetValues(typeof(GametypeHelper.Team)).Length - 1;
                public bool clientTeamChangingLobby = true;
                public bool clientTeamChangingGame = true;
                public bool friendlyFire = false;
                public bool betrayalKicking = false;
            }

            [Serializable]
            public class GeneralOptions
            {
                public int numberOfRounnds = 1;
                public int timeLimitMinutes = 10;
                public int respawnTimeSeconds = 3;
            }

        }
		
		[Server]
        public static void InstanceGametypeByEnum(GametypeHelper.Gametype gametype)
        {
			GameObject gametypeController = Instantiate(GametypeHelper.instance.GetControllerPrefabByGametype(gametype));

			NetworkServer.Spawn(gametypeController);

			NetworkGameManager.instance.activeGametype = gametypeController.GetComponent<GametypeController>();
        }

        public static GameOptions GetGameOptionsByEnum(GametypeHelper.Gametype gametype)
        {
            if (gametype == GametypeHelper.Gametype.Slayer)
                return new SlayerController.SlayerGameOptions();

            else return null;
        }

		[SyncVar]
		public bool hasInitialSpawned = false;
		public virtual IEnumerator InitialSpawnPlayers()
		{
			if (hasInitialSpawned)
			{
				Debug.Log("What are you doing!? Can't initial spawn twice!");
			}
			else
			{
				yield return new WaitForSeconds(10f);
				hasInitialSpawned = true;
				foreach (PlayerData player in NetworkGameManager.instance.Players)
				{
					//Send an RPC to the client who owns this player. Tell them to spawn.
					player.GetComponent<NetworkPlayerSetup>().TargetSetupLocalControl(player.connectionToClient);
				}
			}
		}

		public bool isGameEnding = false;
		public virtual IEnumerator GameOver()
		{
			isGameEnding = true;
			RpcForceOpenScoreboard();
			yield return new WaitForSeconds(5f);
			RpcForceFocusScoreboard();
			yield return new WaitForSeconds(5f);
			isGameEnding = false;
			NetworkGameManager.instance.ServerReturnToLobby();
		}

		[ClientRpc]
		public void RpcForceOpenScoreboard()
		{
			ScoreboardHandler.UpdateHeaderMessage(GetWinner() + " wins!");
			ScoreboardHandler.Open = true;
		}

		[ClientRpc]
		public void RpcForceFocusScoreboard()
		{
			ScoreboardHandler.Focus = true;
		}
	}
}