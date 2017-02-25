using UnityEngine;
using System.Collections;
using System;
using Raider.Game.GUI.Parameters;

namespace Raider.Game.Gametypes
{
    public class Slayer : Gametype
    {
        [Serializable]
        public class SlayerGameOptions : GameOptions
        {
            public SlayerGameOptions()
            {
                scoreToWin = new IntParameter(25, 10, 250);
                gametypeOptions = new SlayerGametypeOptions();
            }

            public class SlayerGametypeOptions : GametypeOptions
            {
                public bool limitedLives = false;
                public IntParameter playerLives = new IntParameter(1, 1, 25);
            }
        }
    }
}