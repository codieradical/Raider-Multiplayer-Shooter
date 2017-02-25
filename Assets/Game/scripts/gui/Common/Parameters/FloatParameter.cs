using System;

namespace Raider.Game.GUI.Parameters
{
    [Serializable]
    public class FloatParameter
    {
        public FloatParameter()
        {
            value = 0;
            minValue = 0;
            maxValue = 0;
        }

        public FloatParameter(float value, float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;

            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            if (value > maxValue || value < minValue)
                throw new ArgumentOutOfRangeException();

            this.value = value;
        }

        protected float value;
        public readonly float minValue;
        public readonly float maxValue;
        public float Value
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
