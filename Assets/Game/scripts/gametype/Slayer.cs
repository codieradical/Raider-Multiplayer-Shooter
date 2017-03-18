using System;

namespace Raider.Game.Gametypes
{
    public class Slayer : Gametype
    {
        [Serializable]
        public class SlayerGameOptions : GameOptions
        {
            public SlayerGameOptions()
            {
                scoreToWin = 50;
                gametypeOptions = new SlayerGametypeOptions();
            }

            public class SlayerGametypeOptions : GametypeOptions
            {
                public bool limitedLives = false;
                public int playerLives = 3;
            }
        }
    }
}