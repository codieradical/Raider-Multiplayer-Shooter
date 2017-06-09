using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.Layout;
using Raider.Game.Saves.User;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Scoreboard
{
	public class ScoreboardPlayerPlate : MonoBehaviour
    {
		public int playerId;
		public GametypeHelper.Team playerTeam;

		bool hasLeft;

        public Text place;
        public EmblemHandler emblem;
        public Text username;
        public Text clan;
        public Text score;
        public List<Image> background;
		public Image gradient;
		public GameObject deadIcon;
		public GameObject leaderIcon;

        public MatchObjectSize matchObjectSizeComponent;

        public void SetupPlate(int playerID, GametypeHelper.Team playerTeam, string place, UserSaveDataStructure.Emblem emblem, string username, string clan, int score, bool hasLeft, Color color, GameObject scoreboardHeader, bool isLeader, bool isDead)
        {
            matchObjectSizeComponent = GetComponent<MatchObjectSize>();

			this.playerId = playerID;
			this.playerTeam = playerTeam;

            this.place.text = place;
            this.emblem.UpdateEmblem(emblem);
            this.username.text = username;
            this.clan.text = clan;
            this.score.text = score.ToString();
            matchObjectSizeComponent.matchGameObject = scoreboardHeader;

			if (isLeader)
				leaderIcon.SetActive(true);

			IsDead = isDead;

			this.hasLeft = hasLeft;

            if (hasLeft)
            {
                Color leftColor = Color.gray;
                this.place.color = leftColor;
                this.username.color = leftColor;
                this.clan.color = leftColor;
                this.score.color = leftColor;
            }

			Material newGradMaterial = new Material(gradient.material);
			float newH, newS, newV;
			Color.RGBToHSV(color, out newH, out newS, out newV);
			float oldH, oldS, oldV, oldA;
			Color newColor = new Color();

			foreach (Image image in background)
            {
                Color.RGBToHSV(image.color, out oldH, out oldS, out oldV);
				oldA = image.color.a;
                newColor = Color.HSVToRGB(newH, newS, oldV);
				image.color = new Color(newColor.r, newColor.g, newColor.b, oldA);
            }

			oldA = newGradMaterial.GetColor("_Color").a;
			Color.RGBToHSV(newGradMaterial.GetColor("_Color"), out oldH, out oldS, out oldV);
			newColor = Color.HSVToRGB(newH, newS, oldV);
			newGradMaterial.SetColor("_Color", new Color(newColor.r, newColor.g, newColor.b, oldA));

			oldA = newGradMaterial.GetColor("_Color2").a;
			Color.RGBToHSV(newGradMaterial.GetColor("_Color2"), out oldH, out oldS, out oldV);
			newColor = Color.HSVToRGB(newH, newS, oldV);
			newGradMaterial.SetColor("_Color2", new Color(newColor.r, newColor.g, newColor.b, oldA));

			gradient.material = newGradMaterial;

		}

		public bool IsDead
		{
			set
			{
				if (hasLeft || playerId == -1)
					deadIcon.SetActive(false);
				else
					deadIcon.SetActive(value);
			}
			get
			{
				return deadIcon.activeSelf;
			}
		}
    }
}