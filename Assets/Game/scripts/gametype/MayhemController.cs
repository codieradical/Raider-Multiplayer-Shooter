using Raider.Game.Player;
using System;

namespace Raider.Game.Gametypes
{
	public class MayhemController : GametypeController
    {

        private void Start()
        {
            NetworkPlayerController.onServerPlayerKilledPlayer += PVPScore;
        }

        [Serializable]
        public class MayhemGameOptions : GameOptions
        {
            public MayhemGameOptions()
            {
                infiniteAmmo = true;
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