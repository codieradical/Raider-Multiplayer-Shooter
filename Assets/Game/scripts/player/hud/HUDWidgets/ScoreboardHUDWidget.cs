using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Raider.Game.Networking;
using Raider.Game.Gametypes;
using Raider.Game.Player;
using Raider.Game.GUI.Scoreboard;

[RequireComponent(typeof(Animator))]
public class ScoreboardHUDWidget : MonoBehaviour
{
    Animator animatorInstance;

    public bool MeLeading
    {
        get { return animatorInstance.GetBool("meLeading"); }
        private set { animatorInstance.SetBool("meLeading", value); }
    }

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

		UpdateWidgetData();
    }

    public void UpdateWidgetData()
    {
        if(NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
        {
            int myTeamScore = GametypeController.singleton.TeamRanking[0].Second;

            Color newColor = GametypeHelper.GetTeamColor(PlayerData.localPlayerData.syncData.team);

            float previousAlpha = myBackground.color.a;
            myBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

            previousAlpha = myMeter.color.a;
            myMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
            myMeter.fillAmount = (float)myTeamScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

            myText.text = myTeamScore.ToString();

            if (GametypeController.singleton.TeamRanking[0].First == PlayerData.localPlayerData.syncData.team)
            {

                MeLeading = true;

                if (GametypeController.singleton.TeamRanking[1] != null)
                {
                    int otherTeamScore = GametypeController.singleton.TeamRanking[1].Second;

                    newColor = GametypeHelper.GetTeamColor(GametypeController.singleton.TeamRanking[1].First);

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

                int otherTeamScore = GametypeController.singleton.TeamRanking[0].Second;

                newColor = GametypeHelper.GetTeamColor(GametypeController.singleton.TeamRanking[0].First);

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
            int myScore = GametypeController.singleton.GetScoreboardPlayerByIDAndTeam(PlayerData.localPlayerData.syncData.id, PlayerData.localPlayerData.syncData.team).score;

            Color newColor = PlayerData.localPlayerData.syncData.Character.armourPrimaryColor.Color;

            float previousAlpha = myBackground.color.a;
            myBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

            previousAlpha = myMeter.color.a;
            myMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
            myMeter.fillAmount = (float)myScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

            myText.text = myScore.ToString();

            if (GametypeController.singleton.PlayerRanking().First[0].id == PlayerData.localPlayerData.syncData.id) {
                MeLeading = true;
                if (GametypeController.singleton.PlayerRanking().First.Count > 1)
                {
                    int otherPlayerScore = GametypeController.singleton.PlayerRanking().First[1].score;

                    newColor = GametypeController.singleton.PlayerRanking().First[1].color;

                    previousAlpha = otherBackground.color.a;
                    otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

                    previousAlpha = myMeter.color.a;
                    otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
                    otherMeter.fillAmount = (float)otherPlayerScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

                    otherText.text = otherPlayerScore.ToString();

                    otherCanvasGroup.alpha = 1;
                }
                else if (GametypeController.singleton.PlayerRanking().First.Count > 0)
                {
                    int otherPlayerScore = GametypeController.singleton.PlayerRanking().First[0].score;

                    newColor = GametypeController.singleton.PlayerRanking().First[0].color;

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
                int otherPlayerScore = GametypeController.singleton.PlayerRanking().First[0].score;

                newColor = GametypeController.singleton.PlayerRanking().First[0].color;

                previousAlpha = otherBackground.color.a;
                otherBackground.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);

                previousAlpha = myMeter.color.a;
                otherMeter.color = new Color(newColor.r, newColor.g, newColor.b, previousAlpha);
                otherMeter.fillAmount = (float)otherPlayerScore / NetworkGameManager.instance.lobbySetup.syncData.gameOptions.scoreToWin;

                otherText.text = otherPlayerScore.ToString();

                otherCanvasGroup.alpha = 1;
            }
        }


        //healthMeter.fillAmount = PlayerData.localPlayerData.networkPlayerController.health / NetworkPlayerController.MAX_HEALTH;
    }
}
