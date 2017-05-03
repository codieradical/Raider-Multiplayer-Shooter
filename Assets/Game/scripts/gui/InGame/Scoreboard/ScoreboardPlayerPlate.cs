using UnityEngine;
using System.Collections;
using Raider.Game.GUI.Components;
using UnityEngine.UI;
using System.Collections.Generic;
using Raider.Game.Saves.User;
using Raider.Game.GUI.Layout;

namespace Raider.Game.GUI.Scoreboard
{
    public class ScoreboardPlayerPlate : MonoBehaviour
    {
        public Text place;
        public EmblemHandler emblem;
        public Text username;
        public Text clan;
        public Text score;
        public List<Image> background;

        public MatchObjectSize matchObjectSizeComponent;

        public void SetupPlate(string place, UserSaveDataStructure.Emblem emblem, string username, string clan, int score, bool hasLeft, Color color, GameObject scoreboardHeader)
        {
            matchObjectSizeComponent = GetComponent<MatchObjectSize>();

            this.place.text = place;
            this.emblem.UpdateEmblem(emblem);
            this.username.text = username;
            this.clan.text = clan;
            this.score.text = score.ToString();
            matchObjectSizeComponent.matchGameObject = scoreboardHeader;

            if (hasLeft)
            {
                Color leftColor = Color.HSVToRGB(1, 1, 0.705f);
                this.place.color = leftColor;
                this.username.color = leftColor;
                this.clan.color = leftColor;
                this.score.color = leftColor;
            }

            foreach (Image image in background)
            {

                float oldH, oldS, oldV;
                Color.RGBToHSV(image.color, out oldH, out oldS, out oldV);
                float newH, newS, newV;
                Color.RGBToHSV(color, out newH, out newS, out newV);

                image.color = Color.HSVToRGB(newH, newS, oldV);
            }

        }
    }
}