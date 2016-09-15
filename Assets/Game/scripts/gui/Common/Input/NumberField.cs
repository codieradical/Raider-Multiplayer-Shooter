using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Raider.Game.GUI.Components
{

    [RequireComponent(typeof(InputField))]
    public class NumberField : MonoBehaviour
    {

        InputField IF;

        [HideInInspector]
        public int value
        {
            get
            {
                if (IF != null)
                    return int.Parse(IF.text);
                else
                    return 0;
            }
            private set
            {
                if (IF != null)
                    IF.text = value.ToString();
            }
        }

        public int min, max;
        public Object[] collection;
        public RangeMode rangeMode;
        public int increaseStep, decreaseStep = 1;

        public enum RangeMode
        {
            Unlimited,
            MinMax,
            Collection
        }

        public void IncreaseValue()
        {
            if (rangeMode == RangeMode.MinMax && (value + increaseStep) >= max)
            {
                value = max;
                return;
            }

            if (rangeMode == RangeMode.Collection && (value + increaseStep) >= collection.Length)
            {
                value = max;
                return;
            }

            value += increaseStep;
        }

        public void DecreaseValue()
        {
            if (rangeMode == RangeMode.MinMax && (value - decreaseStep) <= min)
            {
                value = min;
                return;
            }

            if (rangeMode == RangeMode.Collection && (value - decreaseStep) <= 0)
            {
                value = min;
                return;
            }

            value -= increaseStep;
        }

        // Use this for initialization
        void Start()
        {

            IF = GetComponent<InputField>();

            IF.contentType = InputField.ContentType.IntegerNumber;
        }
    }
}