using Raider.Common.Types;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
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

		void OnDestroy()
		{
			singleton = null;
		}

		#endregion

		#region Scoring

		public struct ScoreboardPlayer
		{
			public ScoreboardPlayer(int id, string name, int score, Gametypes.Teams team)
			{
				this.id = id;
				this.name = name;
				this.score = score;
				this.team = team;
			}
			public int id;
			public string name;
			public int score;
			public Gametypes.Teams team;
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
				if (player.id == playerData.syncData.id && player.team == playerData.syncData.team)
				{
					return;
				}
			}

			scoreboard.Add(new ScoreboardPlayer(playerData.syncData.id, playerData.syncData.username, 0, playerData.syncData.team));
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

		public List<Tuple<Gametypes.Teams, int>> teamScores
		{
			get
			{
				List<Tuple<Gametypes.Teams, int>> teamScores = new List<Tuple<Gametypes.Teams, int>>();
				foreach (Gametypes.Teams team in Enum.GetValues(typeof(Gametypes.Teams)))
				{
					int iterationScore = 0;
					foreach (ScoreboardPlayer player in scoreboard)
					{
						if (player.team == team)
							iterationScore += player.score;
					}
					teamScores.Add(new Tuple<Gametypes.Teams, int>(team, iterationScore));
				}
				return teamScores;
			}
		}

		[Server]
		public void AddToScoreboard(int playerId, Gametypes.Teams playerTeam, int addition)
		{
			for (int i = 0; i < scoreboard.Count; i++)
			{
				if (scoreboard[i].id == playerId && scoreboard[i].team == playerTeam)
				{
					ScoreboardPlayer updatedScore = new ScoreboardPlayer(scoreboard[i].id, scoreboard[i].name, scoreboard[i].score + addition, scoreboard[i].team);
					scoreboard[i] = updatedScore;
					return;
				}
			}
			ScoreboardPlayer newScore = new ScoreboardPlayer(playerId, NetworkGameManager.instance.GetPlayerDataById(playerId).syncData.username, addition, playerTeam);
			scoreboard.Add(newScore);
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
                public int maxTeams = Enum.GetValues(typeof(Gametypes.Teams)).Length - 1;
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
        public static void InstanceGametypeByEnum(Gametypes.Gametype gametype)
        {
			GameObject gametypeController = Instantiate(Gametypes.instance.GetControllerPrefabByGametype(gametype));

			NetworkServer.Spawn(gametypeController);

			NetworkGameManager.instance.activeGametype = gametypeController.GetComponent<GametypeController>();
        }

        public static GameOptions GetGameOptionsByEnum(Gametypes.Gametype gametype)
        {
            if (gametype == Gametypes.Gametype.Slayer)
                return new Slayer.SlayerGameOptions();

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
	}
}