using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Raider.Game.GUI.Parameters
{
    [Serializable]
    public class IntParameter
    {
        public IntParameter()
        {
            minValue = 0;
            maxValue = 0;
            value = 0;
        }

        public IntParameter(int value, int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;

            if(minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            if (value > maxValue || value < minValue)
                throw new ArgumentOutOfRangeException();

            this.value = value;
        }

        protected int value;
        public readonly int minValue;
        public readonly int maxValue;
        public int Value
        {
            get { return value; }
            set
            {
                if (value > maxValue || value < minValue)
                    return;
                this.value = value;
            }
        }
    }
}
