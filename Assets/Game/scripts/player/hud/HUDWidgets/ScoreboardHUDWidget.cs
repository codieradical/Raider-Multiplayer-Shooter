using Raider.Common.Types;
using Raider.Game.Gametypes;
using Raider.Game.GUI.Scoreboard;
using Raider.Game.Networking;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.Player.HUD
{

    [RequireComponent(typeof(Animator))]
    public class ScoreboardHUDWidget : HUDWidget
    {
        Animator animatorInstance;

        public bool MeLeading
        {
            get { return animatorInstance.GetBool("meLeading"); }
            private set { animatorInstance.SetBool("meLeading", value); }
        }

        [Header("Scoreboard Header")]
        public Text timeRemaining;
        public Text gametype;

        [Header("My Score")]
        public Image myBackground;
        public Image myMeter;
        public Text myText;

        [Header("Other Score")]
        public CanvasGroup otherCanvasGroup;
        public Image otherBackground;
        public Image otherMeter;
        public Text otherText;

        private void Start()
        {
            ScoreboardHandler.scoreboardHUDInvalidate = UpdateWidgetData;

            animatorInstance = GetComponent<Animator>();

            gametype.text = NetworkGameManager.instance.lobbySetup.syncData.GametypeString;
            if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
                gametype.text = "Team " + gametype.text;

            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.TimeLimit)
                timeRemaining.text = "";

            UpdateWidgetData();
        }

        public void UpdateWidgetData()
        {
			try
			{
				if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
				{
					int myTeamScore = 0;

					foreach (Tuple<GametypeHelper.Team, int> team in GametypeController.singleton.TeamRanking)
					{
						if (team.Item1 == PlayerData.localPlayerData.syncData.team)
							myTeamScore = team.Item2;
					}

					Color newColor = GametypeHelper.GetTeamColor(PlayerData.localPlayerData.syncData.team);

					float previousAlpha = myBackground.color.a;
					myBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

					previousAlpha = myMeter.color.a;
					myMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
					myMeter.fillAmount = (float)myTeamScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

					myText.text = myTeamScore.ToString();

					if (GametypeController.singleton.TeamRanking[0].Item1 == PlayerData.localPlayerData.syncData.team)
					{

						MeLeading = true;

						if (GametypeController.singleton.TeamRanking.Count > 1)
						{
							int otherTeamScore = GametypeController.singleton.TeamRanking[1].Item2;

							newColor = GametypeHelper.GetTeamColor(GametypeController.singleton.TeamRanking[1].Item1);

							previousAlpha = otherBackground.color.a;
							otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

							previousAlpha = myMeter.color.a;
							otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
							otherMeter.fillAmount = (float)otherTeamScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

							otherText.text = otherTeamScore.ToString();

							otherCanvasGroup.alpha = 1;
						}
						else
						{
							otherCanvasGroup.alpha = 0;
						}
					}
					else
					{
						MeLeading = false;

						int otherTeamScore = GametypeController.singleton.TeamRanking[0].Item2;

						newColor = GametypeHelper.GetTeamColor(GametypeController.singleton.TeamRanking[0].Item1);

						previousAlpha = otherBackground.color.a;
						otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

						previousAlpha = myMeter.color.a;
						otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
						otherMeter.fillAmount = (float)otherTeamScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

						otherText.text = otherTeamScore.ToString();

						otherCanvasGroup.alpha = 1;
					}
				}
				else
				{
					int myScore = 0;

					try
					{
						myScore = GametypeController.singleton.GetScoreboardPlayerByIDAndTeam(PlayerData.localPlayerData.syncData.id, PlayerData.localPlayerData.syncData.team).score;
					}
					catch (NullReferenceException)
					{
						return;
					}

					Color newColor = PlayerData.localPlayerData.syncData.Character.armourPrimaryColor.Color;

					float previousAlpha = myBackground.color.a;
					myBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

					previousAlpha = myMeter.color.a;
					myMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
					myMeter.fillAmount = (float)myScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

					myText.text = myScore.ToString();

					if (GametypeController.singleton.PlayerRanking().Item1[0].id == PlayerData.localPlayerData.syncData.id)
					{
						MeLeading = true;
						if (GametypeController.singleton.PlayerRanking().Item1.Count > 1)
						{
							int otherPlayerScore = GametypeController.singleton.PlayerRanking().Item1[1].score;

							newColor = GametypeController.singleton.PlayerRanking().Item1[1].color;

							previousAlpha = otherBackground.color.a;
							otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

							previousAlpha = myMeter.color.a;
							otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
							otherMeter.fillAmount = (float)otherPlayerScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

							otherText.text = otherPlayerScore.ToString();

							otherCanvasGroup.alpha = 1;
						}
						else if (GametypeController.singleton.PlayerRanking().Item2.Count > 0)
						{
							int otherPlayerScore = GametypeController.singleton.PlayerRanking().Item1[0].score;

							newColor = GametypeController.singleton.PlayerRanking().Item1[0].color;

							previousAlpha = otherBackground.color.a;
							otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

							previousAlpha = myMeter.color.a;
							otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
							otherMeter.fillAmount = (float)otherPlayerScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

							otherText.text = otherPlayerScore.ToString();

							otherCanvasGroup.alpha = 1;
						}
						else
						{
							otherCanvasGroup.alpha = 0;
						}
					}
					else
					{
						MeLeading = false;
						int otherPlayerScore = GametypeController.singleton.PlayerRanking().Item1[0].score;

						newColor = GametypeController.singleton.PlayerRanking().Item1[0].color;

						previousAlpha = otherBackground.color.a;
						otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

						previousAlpha = myMeter.color.a;
						otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
						otherMeter.fillAmount = (float)otherPlayerScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

						otherText.text = otherPlayerScore.ToString();

						otherCanvasGroup.alpha = 1;
					}
				}
			}
			catch(NullReferenceException)
			{
				Debug.Log("Can't update scoreboard before data initialized.");	
			}


            //healthMeter.fillAmount = PlayerData.localPlayerData.networkPlayerController.health / NetworkPlayerController.MAX_HEALTH;
        }

        private void UpdateTimeRemaining()
        {
            if (GametypeController.singleton != null && GametypeController.singleton.hasInitialSpawned && !GametypeController.singleton.isGameEnding)
            {
                TimeSpan span = new TimeSpan(0, 0, (int)(GametypeController.singleton.gameEnds - NetworkGameManager.syncServerTime));
                timeRemaining.text = string.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
            }
            else
                timeRemaining.text = "0:00";
        }

        private void Update()
        {
            if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.TimeLimit)
                UpdateTimeRemaining();
        }
    }
}