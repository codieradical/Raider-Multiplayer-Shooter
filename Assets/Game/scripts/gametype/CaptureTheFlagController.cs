using Raider.Game.Networking;
using Raider.Game.Player;
using System;

namespace Raider.Game.Gametypes
{
    public class CaptureTheFlagController : GametypeController
    {
        private void Start()
        {
            NetworkPlayerController.onServerPlayerScored += GametypeScore;
        }

        [Serializable]
        public class CaptureTheFlagGameOptions : GameOptions
        {
            public CaptureTheFlagGameOptions()
            {
                forceTeams = true;
                teamsEnabled = true;
                scoreToWin = 3;
                gametypeOptions = new CaptureTheFlagGametypeOptions();
			}

            public class CaptureTheFlagGametypeOptions : GametypeOptions
            {
                //public bool limitedLives = false;
                //public int playerLives = 3;
            }
        }
    }
}