using Raider.Game.GUI.Layout;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Scoreboard
{
	public class ScoreboardTeamPlate : MonoBehaviour
    {
        public Text place;
        public Text teamname;
        public Text score;
        public List<Image> background;

        public MatchObjectSize matchObjectSizeComponent;

        public void SetupPlate(string place, string teamname, int score, Color color, GameObject scoreboardHeader, bool hasLeft)
        {
            matchObjectSizeComponent = GetComponent<MatchObjectSize>();

            this.place.text = place;
            this.teamname.text = teamname;
            this.score.text = score.ToString();

            matchObjectSizeComponent.matchGameObject = scoreboardHeader;

			float newH, newS, newV;
			Color.RGBToHSV(color, out newH, out newS, out newV);
			Color newColor;

			if (hasLeft)
			{
				Color leftColor = Color.gray;
				this.place.color = leftColor;
				this.teamname.color = leftColor;
				this.score.color = leftColor;
			}

			foreach (Image image in background)
            {
                float oldH, oldS, oldV, oldA;
                Color.RGBToHSV(image.color, out oldH, out oldS, out oldV);
				oldA = image.color.a;
                newColor = Color.HSVToRGB(newH, newS, oldV);
				image.color = new Color(newColor.r, newColor.g, newColor.b, oldA);
            }

        }
    }
}