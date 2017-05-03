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
			public ScoreboardPlayer(int id, int score, GametypeHelper.Teams team, string name, string clan, UserSaveDataStructure.Emblem emblem)
			{
				this.id = id;
				this.score = score;
				this.team = team;
                this.name = name;
                this.clan = clan;
                this.emblem = emblem;
			}
			public int id;
            public string name;
            public string clan;
            public UserSaveDataStructure.Emblem emblem;
			public int score;
			public GametypeHelper.Teams team;
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

			scoreboard.Add(new ScoreboardPlayer(playerData.syncData.id, 0, playerData.syncData.team, playerData.syncData.username, playerData.syncData.Character.guild, playerData.syncData.Character.emblem));
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

		public List<Tuple<GametypeHelper.Teams, int>> teamRanking
		{
			get
			{
				List<Tuple<GametypeHelper.Teams, int>> teamScores = new List<Tuple<GametypeHelper.Teams, int>>();
				foreach (GametypeHelper.Teams team in Enum.GetValues(typeof(GametypeHelper.Teams)))
				{
					int iterationScore = 0;
					foreach (ScoreboardPlayer player in scoreboard)
					{
						if (player.team == team)
							iterationScore += player.score;
					}
					teamScores.Add(new Tuple<GametypeHelper.Teams, int>(team, iterationScore));
				}
				return (teamScores.OrderBy(team => PlayerRanking(team.First).Count() < 1).ThenBy(team => team.Second).ThenBy(team => team.First)).ToList();
			}
		}

        public List<ScoreboardPlayer> PlayerRanking(GametypeHelper.Teams team)
        {
            List<ScoreboardPlayer> players = new List<ScoreboardPlayer>();

            foreach (ScoreboardPlayer player in scoreboard)
                if (player.team == team)
                    players.Add(player);

            return (players.OrderBy(player => NetworkGameManager.instance.GetPlayerDataById(player.id) != null).ThenBy(player => player.score).ThenBy(player => player.id)).ToList();
        }

        public List<ScoreboardPlayer> PlayerRanking()
        {
            return (scoreboard.OrderBy(player => NetworkGameManager.instance.GetPlayerDataById(player.id) != null).ThenBy(player => player.score).ThenBy(player => player.id)).ToList();
        }

		[Server]
		public void AddToScoreboard(int playerId, GametypeHelper.Teams playerTeam, int addition)
		{
			for (int i = 0; i < scoreboard.Count; i++)
			{
				if (scoreboard[i].id == playerId && scoreboard[i].team == playerTeam)
				{
					ScoreboardPlayer updatedScore = new ScoreboardPlayer(scoreboard[i].id, scoreboard[i].score + addition, scoreboard[i].team, scoreboard[i].name, scoreboard[i].clan, scoreboard[i].emblem);
					scoreboard[i] = updatedScore;
					return;
				}
			}
            PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerId);
            ScoreboardPlayer newScore = new ScoreboardPlayer(playerId, addition, playerTeam, playerData.syncData.username, playerData.syncData.Character.guild, playerData.syncData.Character.emblem);
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
                public int maxTeams = Enum.GetValues(typeof(GametypeHelper.Teams)).Length - 1;
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
	}
}