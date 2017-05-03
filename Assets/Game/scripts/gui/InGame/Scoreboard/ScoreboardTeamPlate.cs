using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Raider.Game.GUI.Layout;

namespace Raider.Game.GUI.Scoreboard
{
    public class ScoreboardTeamPlate : MonoBehaviour
    {
        public Text place;
        public Text teamname;
        public Text score;
        public List<Image> background;

        public MatchObjectSize matchObjectSizeComponent;

        public void SetupPlate(string place, string teamname, int score, Color color, GameObject scoreboardHeader)
        {
            matchObjectSizeComponent = GetComponent<MatchObjectSize>();

            this.place.text = place;
            this.teamname.text = teamname;
            this.score.text = score.ToString();

            matchObjectSizeComponent.matchGameObject = scoreboardHeader;

			float newH, newS, newV;
			Color.RGBToHSV(color, out newH, out newS, out newV);
			Color newColor;

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