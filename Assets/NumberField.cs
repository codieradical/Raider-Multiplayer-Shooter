using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(InputField))]
public class NumberField : MonoBehaviour {

    InputField IF;

    int value { get { return int.Parse(IF.text); } set { IF.text = value.ToString(); } }

    public int min, max;
    public Object[] collection;
    public RangeMode rangeMode;
    public int increaseStep, decreaseStep;

    public enum RangeMode
    {
        Unlimited,
        MinMax,
        Collection
    }

    public void IncreaseValue()
    {
        if(rangeMode == RangeMode.MinMax)
        {
            if ((value + increaseStep) >= max)
                value = max;

            if ((value - increaseStep) <= min)
                value = min;
        }

        if (rangeMode == RangeMode.Collection)
        {
            if ((value + increaseStep) >= collection.Length)
                value = max;

            if ((value - increaseStep) <= 0)
                value = min;
        }
    }

    public void DecreaseValue()
    {

    }

	// Use this for initialization
	void Start () {

	    IF = GetComponent<InputField>();

        IF.contentType = InputField.ContentType.IntegerNumber;

    }
	
}
