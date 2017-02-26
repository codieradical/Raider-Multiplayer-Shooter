using UnityEngine;
using System;
using Raider.Game.Scene;

namespace Raider.Game.Gametypes
{
    public abstract class Gametype : MonoBehaviour
    {
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
                public int maxTeams = Enum.GetValues(typeof(Teams)).Length + 1;
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

        public static Gametype GetGametypeByEnum(Scenario.Gametype gametype)
        {
            if (gametype == Scenario.Gametype.Slayer)
                return new Slayer();

            else return null;
        }

        public static GameOptions GetGameOptionsByEnum(Scenario.Gametype gametype)
        {
            if (gametype == Scenario.Gametype.Slayer)
                return new Slayer.SlayerGameOptions();

            else return null;
        }

        public static Color GetTeamColor(Teams team)
        {
            switch(team)
            {
                case Teams.Blue:
                    return Color.blue;
                case Teams.Brown:
                    return new Color(0.54f, 0.27f, 0.07f);
                case Teams.Green:
                    return Color.green;
                case Teams.Pink:
                    return new Color(0.97f, 1f, 0.86f);
                case Teams.Purple:
                    return Color.magenta;
                case Teams.Red:
                    return Color.red;
                case Teams.White:
                    return Color.white;
                case Teams.Yellow:
                    return Color.yellow;
                case Teams.Black:
                    return Color.black;
            }

            Debug.Log("Team color out of enum!"); // Could probably default case this.
            return Color.black;
        }

        public enum Teams
        {
            None = 0,
            Red = 1,
            Blue = 2,
            Green = 3,
            Yellow = 4,
            Pink = 5,
            Brown = 6,
            Purple = 7,
            White = 8,
            Black = 9
        }
    }
}