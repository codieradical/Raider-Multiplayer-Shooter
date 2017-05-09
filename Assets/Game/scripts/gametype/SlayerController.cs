using Raider.Game.Player;
using System;

namespace Raider.Game.Gametypes
{
    public class SlayerController : GametypeController
    {
        private void Start()
        {
            NetworkPlayerController.onServerPlayerKilledPlayer += PVPScore;
        }

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