using System;

namespace Raider.Game.Gametypes
{
	public class KingOfTheHillController : GametypeController
    {
        private void Start()
        {
            HillObjective.timerScore += GametypeScore;
        }

        [Serializable]
        public class KingOfTheHillGameOptions : GameOptions
        {
            public KingOfTheHillGameOptions()
            {
                teamsEnabled = true;
                scoreToWin = 3;
                gametypeOptions = new KingOfTheHillGametypeOptions();
            }

            public class KingOfTheHillGametypeOptions : GametypeOptions
            {
                //public bool limitedLives = false;
                //public int playerLives = 3;
            }
        }
    }
}