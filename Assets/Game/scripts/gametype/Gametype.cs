using UnityEngine;
using System;
using Raider.Game.GUI.Parameters;
using Raider.Game.Scene;

namespace Raider.Game.Gametypes
{
    public abstract class Gametype : MonoBehaviour
    {
        [Serializable]
        public class GameOptions
        {
            public IntParameter scoreToWin = new IntParameter(25, 10, 1000);
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
                public bool teamsEnabled;
                public IntParameter maxTeams = new IntParameter( //The max amount of teams is the number of teams. The default value is the max. The minimum is 2.
                    Enum.GetValues(typeof(Networking.LobbySetup.Teams)).Length + 1, 2, Enum.GetValues(typeof(Networking.LobbySetup.Teams)).Length + 1);
                public bool clientTeamChangingLobby;
                public bool clientTeamChangingGame;
                public bool friendlyFire;
                public bool betrayalKicking;
            }

            [Serializable]
            public class GeneralOptions
            {
                public IntParameter numberOfRounnds = new IntParameter(1, 1, 10);
                public IntParameter timeLimitMinutes = new IntParameter(5, 1, 60);
                public IntParameter respawnTimeSeconds = new IntParameter(3, 1, 30);
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
    }
}