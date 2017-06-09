using System;

namespace Raider.Game.Gametypes
{
	public class OddballController : GametypeController
    {
        private void Start()
        {
            OddballObjective.timerScore += GametypeScore;
        }

        [Serializable]
        public class OddballGameOptions : GameOptions
        {
            public OddballGameOptions()
            {
                teamsEnabled = true;
                scoreToWin = 3;
                gametypeOptions = new OddballGametypeOptions();
            }

            public class OddballGametypeOptions : GametypeOptions
            {
                //public bool limitedLives = false;
                //public int playerLives = 3;
            }
        }
    }
}