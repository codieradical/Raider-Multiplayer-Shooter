﻿using Raider.Common.Types;
using Raider.Game.GUI.Scoreboard;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Saves.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

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
		}

		private void Start()
		{
			if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Server || NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host)
				ScoreboardHandler.InvalidateScoreboard();
		}

		void OnDestroy()
		{
			singleton = null;
		}

		#endregion

		#region Scoring

		/// <summary>
		/// Scoreboard entry structure.
		/// </summary>
		public struct ScoreboardPlayer
		{
			public ScoreboardPlayer(int id, int score, GametypeHelper.Team team, string name, string clan, Color color, UserSaveDataStructure.Emblem emblem)
			{
				this.id = id;
				this.score = score;
				this.team = team;
				this.name = name;
				this.clan = clan;
				this.emblem = emblem;
				this.color = color;
				created = Time.time;
			}
			public int id;
			public string name;
			public string clan;
			public UserSaveDataStructure.Emblem emblem;
			public int score;
			public GametypeHelper.Team team;
			public Color color;
			public float created;
		}

		/// <summary>
		/// SyncList class deriving from SyncListStruct.
		/// </summary>
		public class SyncListScoreboardPlayer : SyncListStruct<ScoreboardPlayer>
		{

		}

		/// <summary>
		/// Scoreboard containing active players.
		/// </summary>
		public SyncListScoreboardPlayer scoreboard = new SyncListScoreboardPlayer();
		/// <summary>
		/// Scoreboard containing players who have left, or switched teams.
		/// </summary>
		public SyncListScoreboardPlayer inactiveScoreboard = new SyncListScoreboardPlayer();

		/// <summary>
		/// If the scoreboard changes, log the change.
		/// </summary>
		public void OnScoreboardChanged(SyncListScoreboardPlayer.Operation operation, int index)
		{
			Debug.Log(operation.ToString() + " at " + index.ToString());
			//Invalidate the scoreboard GUI.
			ScoreboardHandler.InvalidateScoreboard();
		}

		public override void OnStartClient()
        {
			Debug.Log("Added invalidatescoreboard callback");
            scoreboard.Callback = OnScoreboardChanged;
            inactiveScoreboard.Callback = OnScoreboardChanged;

			//if (!NetworkServer.active)
				OnScoreboardChanged(SyncList<ScoreboardPlayer>.Operation.OP_DIRTY, 0);

            //NetworkGameManager.instance.onLobbyServerDisconnect += ScoreboardHandler.InvalidateScoreboard;
        }

		[Server]
        public void InactivateScoreboardPlayer(int id, GametypeHelper.Team team)
        {
            foreach(ScoreboardPlayer player in scoreboard)
            {
                if(player.id == id && player.team == team)
                {
                    bool addPlayer = true;

                    foreach(ScoreboardPlayer inactivePlayer in inactiveScoreboard)
                    {
                        if (inactivePlayer.id == id && inactivePlayer.team == team)
                        {
                            Debug.LogWarning("Inactive scoreboard already contains this player!");
                            addPlayer = false;
                        }
                    }

                    if(addPlayer)
                        inactiveScoreboard.Add(player);

                    scoreboard.Remove(player);
                }
            }
        }

		[Server]
        public void AddOrReactivateScoreboardPlayer(int id, GametypeHelper.Team team)
        {
            foreach (ScoreboardPlayer player in inactiveScoreboard)
            {
                if (player.id == id && player.team == team)
                {
                    scoreboard.Add(player);
                    inactiveScoreboard.Remove(player);
					return;
                }
            }

            foreach (ScoreboardPlayer player in scoreboard)
            {
                if(player.id == id)
                {
                    Debug.Log("Attempting to add player " + player.name + " to the scoreboard again...");
                    if (player.team != team)
                        InactivateScoreboardPlayer(id, player.team);
                    else
                        return;
                }
            }

            PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(id);
            scoreboard.Add(new ScoreboardPlayer(id, 0, team, playerData.syncData.username, playerData.syncData.Character.guild, playerData.syncData.Character.armourPrimaryColor.Color, playerData.syncData.Character.emblem));
		}

		[Server]
		public void ChangeScoreboardPlayerTeam(int playerID, GametypeHelper.Team newTeam)
		{
			ScoreboardPlayer? oldActivePlayer = null;
			ScoreboardPlayer? oldInactivePlayer = null;

			int oldActivePlayerIndex = 0;
			int oldInactivePlayerIndex = 0;

			//If the player was previously on the team they're changing too, grab their old entry.
			for (int i = 0; i < inactiveScoreboard.Count; i++)
			{
				if(inactiveScoreboard[i].id == playerID && inactiveScoreboard[i].team == newTeam)
				{
					oldInactivePlayerIndex = i;
					oldInactivePlayer = inactiveScoreboard[i];
					break;
				}
			}

			for (int i = 0; i < scoreboard.Count; i++)
			{
				if (scoreboard[i].id == playerID)
				{
					oldActivePlayerIndex = i;
					oldActivePlayer = scoreboard[i];
					break;
				}
			}

			//If the player was previously inactive on the team they're switching to,
			//Swap the inactive and active entries.
			if (oldInactivePlayer != null)
			{
				inactiveScoreboard[oldInactivePlayerIndex] = (ScoreboardPlayer)oldActivePlayer;
				scoreboard[oldActivePlayerIndex] = (ScoreboardPlayer)oldInactivePlayer;
			}
			else
			{
				inactiveScoreboard.Add((ScoreboardPlayer)oldActivePlayer);
				PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerID);
				ScoreboardPlayer newPlayer = new ScoreboardPlayer(playerID, 0, newTeam, playerData.PlayerSyncData.username, playerData.PlayerSyncData.Character.guild, playerData.PlayerSyncData.Character.armourPrimaryColor.Color, playerData.PlayerSyncData.Character.emblem);
				scoreboard[oldActivePlayerIndex] = newPlayer;
			}
		}

		public List<Tuple<GametypeHelper.Team, int>> TeamRanking
		{
			get
			{
				List<Tuple<GametypeHelper.Team, int>> teamScores = new List<Tuple<GametypeHelper.Team, int>>();
				foreach (GametypeHelper.Team team in Enum.GetValues(typeof(GametypeHelper.Team)))
				{
					if (PlayerRanking(team).Item1.Concat(PlayerRanking(team).Item2).Count() < 1)
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

                    foreach (ScoreboardPlayer player in inactiveScoreboard)
                    {
                        if (player.team == team)
                            iterationScore += player.score;
                    }
                    teamScores.Add(new Tuple<GametypeHelper.Team, int>(team, iterationScore));
				}
				return (teamScores.OrderBy(team => PlayerRanking(team.Item1).Item1.Concat(PlayerRanking(team.Item1).Item2).Count() < 1).ThenByDescending(team => team.Item2).ThenBy(team => team.Item1)).ToList();
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

            foreach (ScoreboardPlayer player in inactiveScoreboard)
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

        public Tuple<List<ScoreboardPlayer>, List<ScoreboardPlayer>> PlayerRanking(GametypeHelper.Team team)
        {
            List<ScoreboardPlayer> players = new List<ScoreboardPlayer>();

			foreach (ScoreboardPlayer player in scoreboard)
			{
				if (player.team == team)
				{
					players.Add(player);
					continue;
				}
			}

            players = (players.OrderByDescending(player => player.score).ThenBy(player => player.id)).ToList();

            List<ScoreboardPlayer> inactivePlayers = new List<ScoreboardPlayer>();

            foreach (ScoreboardPlayer inactivePlayer in inactiveScoreboard)
            {
                bool addPlayer = false;

                if (inactivePlayer.team == team)
                {
                    addPlayer = true;
                    //continue;
                }

                //If the player is active, don't show their old inactive lir.
                foreach (ScoreboardPlayer player in scoreboard)
                {
                    if (inactivePlayer.id == player.id)
                    {
                        addPlayer = false;
                        break;
                    }
                }

                //If the player is inactive in another team, make sure the most recent is shown.
                foreach (ScoreboardPlayer inactivePlayer2 in scoreboard)
                {
                    if (inactivePlayer2.created > inactivePlayer.created)
                    {
                        addPlayer = false;
                        break;
                    }
                }

                if (addPlayer)
                    inactivePlayers.Add(inactivePlayer);
            }

            inactivePlayers = (inactivePlayers.OrderByDescending(inactivePlayer => inactivePlayer.score).ThenBy(inactivePlayer => inactivePlayer.id)).ToList();

            return new Tuple<List<ScoreboardPlayer>, List<ScoreboardPlayer>>(players, inactivePlayers);
        }

        public Tuple<List<ScoreboardPlayer>, List<ScoreboardPlayer>> PlayerRanking()
        {
            List<ScoreboardPlayer> players = new List<ScoreboardPlayer>();
            players = (scoreboard.OrderByDescending(player => player.score).ThenBy(player => player.id)).ToList();

            List<ScoreboardPlayer> inactivePlayers = new List<ScoreboardPlayer>();
            inactivePlayers = (inactiveScoreboard.OrderByDescending(player => player.score).ThenBy(player => player.id)).ToList();

            return new Tuple<List<ScoreboardPlayer>, List<ScoreboardPlayer>>(players, inactivePlayers);
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
					ScoreboardPlayer updatedScore = new ScoreboardPlayer(scoreboard[i].id, scoreboard[i].score + addition, scoreboard[i].team, scoreboard[i].name, scoreboard[i].clan, scoreboard[i].color, scoreboard[i].emblem);
					scoreboard[i] = updatedScore;
					//scoreboard.RemoveAt(i);
					//scoreboard.Add(updatedScore);

					scoreboard.Dirty(i);

                    if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
                    {
                        if (TeamRanking[0].Item2 >= NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin)
                            StartCoroutine(GameOver());
                    }
                    else if (updatedScore.score >= NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin)
                        StartCoroutine(GameOver());

                    return;
				}
			}
			PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerId);
			ScoreboardPlayer newScore = new ScoreboardPlayer(playerId, addition, playerTeam, playerData.PlayerSyncData.username, playerData.PlayerSyncData.Character.guild, playerData.PlayerSyncData.Character.armourPrimaryColor.Color, playerData.PlayerSyncData.Character.emblem);
			scoreboard.Add(newScore);

			if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
			{
				if (TeamRanking[0].Item2 >= NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin)
					StartCoroutine(GameOver());
			}
			else if (newScore.score >= NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin)
				StartCoroutine(GameOver());
		}

        public ScoreboardPlayer GetScoreboardPlayerByIDAndTeam(int playerID, GametypeHelper.Team team)
        {
            foreach(ScoreboardPlayer player in scoreboard)
            {
                if (player.id == playerID && player.team == team)
                    return player;
            }

            foreach (ScoreboardPlayer player in inactiveScoreboard)
            {
                if (player.id == playerID && player.team == team)
                    return player;
            }

            return new ScoreboardPlayer();
        }

		public string GetWinner()
		{
			if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
				return TeamRanking[0].Item1.ToString();
			else
				return PlayerRanking().Item1[0].name;
		}

		#endregion

		#region Game Options

		[Serializable]
        public class GameOptions
        {
            //HIDDEN FIELDS
            public bool infiniteAmmo = false;
            public bool forceTeams = false;


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
                public bool TimeLimit
                {
                    get
                    {
                        if (timeLimitMinutes < 0)
                            return false;
                        else
                            return true;
                    }
                }
            }

        }

#endregion

		[Server]
        public static void InstanceGametypeByEnum(GametypeHelper.Gametype gametype)
        {
			GameObject gametypeController = Instantiate(GametypeHelper.instance.GetControllerPrefabByGametype(gametype));

			NetworkServer.Spawn(gametypeController);

			NetworkGameManager.instance.activeGametype = gametypeController.GetComponent<GametypeController>();
        }

        public static GameOptions GetGameOptionsByEnum(GametypeHelper.Gametype gametype)
        {
            switch (gametype)
            {
                case GametypeHelper.Gametype.Mayhem:
                    return new MayhemController.MayhemGameOptions();
                case GametypeHelper.Gametype.Capture_The_Flag:
                    return new CaptureTheFlagController.CaptureTheFlagGameOptions();
                case GametypeHelper.Gametype.King_Of_The_Hill:
                    return new KingOfTheHillController.KingOfTheHillGameOptions();
                case GametypeHelper.Gametype.Oddball:
                    return new OddballController.OddballGameOptions();
                case GametypeHelper.Gametype.Slayer:
                    return new SlayerController.SlayerGameOptions();
            }

            return null;
        }

		#region Initial Spawn and Game Over

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
                if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.TimeLimit)
                    gameEnds = (float)NetworkGameManager.syncServerTime + (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.timeLimitMinutes * 60);
                hasInitialSpawned = true;
                foreach (PlayerData player in NetworkGameManager.instance.Players)
				{
					//Send an RPC to the client who owns this player. Tell them to spawn.
					player.GetComponent<NetworkPlayerSetup>().TargetSetupLocalControl(player.connectionToClient);
				}
			}
		}

		public bool isGameEnding = false;
		[Server]
		protected virtual IEnumerator GameOver()
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
		private void RpcForceOpenScoreboard()
		{
			ScoreboardHandler.UpdateHeaderMessage(GetWinner() + " wins!");
			ScoreboardHandler.Open = true;
		}

		[ClientRpc]
		private void RpcForceFocusScoreboard()
		{
			ScoreboardHandler.Focus = true;
		}

        [Server]
        protected virtual void PVPScore(int killed, int killedby)
        {
            PlayerData.SyncData killerPlayer = NetworkGameManager.instance.GetPlayerDataById(killedby).syncData;
            PlayerData.SyncData killedPlayer = NetworkGameManager.instance.GetPlayerDataById(killed).syncData;

            if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled && killerPlayer.team == killedPlayer.team)
                return;
    
            AddToScoreboard(killerPlayer.id, killerPlayer.team, 1);
        }

        [Server]
        protected virtual void GametypeScore(int scoredBy)
        {
            PlayerData.SyncData scoringPlayer = NetworkGameManager.instance.GetPlayerDataById(scoredBy).syncData;

            AddToScoreboard(scoringPlayer.id, scoringPlayer.team, 1);
        }

        [SyncVar]
        public float gameEnds;
        /// <summary>
        /// Checks if the game end time is in the past.
        /// Don't call this if there's no time limit.
        /// </summary>
		[Server]
        protected void CheckGameTime()
        {
            if (!hasInitialSpawned || isGameEnding)
                return;

            if (NetworkGameManager.syncServerTime > gameEnds)
                StartCoroutine(GameOver());
        }

        protected void Update()
        {
            if(NetworkServer.active && NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.TimeLimit)
                CheckGameTime();
        }

		#endregion
	}
}