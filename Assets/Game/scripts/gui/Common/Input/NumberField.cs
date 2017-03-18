using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{

    [RequireComponent(typeof(InputField))]
    public class NumberField : MonoBehaviour
    {

        InputField IF;

        [HideInInspector]
        public int Value
        {
            get
            {
                if (IF != null)
                    return int.Parse(IF.text);
                else
                    return 0;
            }
            set
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
            if (rangeMode == RangeMode.MinMax && (Value + increaseStep) >= max)
            {
                Value = max;
                return;
            }

            if (rangeMode == RangeMode.Collection && (Value + increaseStep) >= collection.Length)
            {
                Value = max;
                return;
            }

            Value += increaseStep;
        }

        public void DecreaseValue()
        {
            if (rangeMode == RangeMode.MinMax && (Value - decreaseStep) <= min)
            {
                Value = min;
                return;
            }

            if (rangeMode == RangeMode.Collection && (Value - decreaseStep) <= 0)
            {
                Value = min;
                return;
            }

            Value -= increaseStep;
        }

        // Use this for initialization
        void Start()
        {

            IF = GetComponent<InputField>();

            IF.contentType = InputField.ContentType.IntegerNumber;
        }
    }
}