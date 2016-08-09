using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HSLColorPicker : MonoBehaviour {

    int h
    {
        get
        {
            try
            {
                return int.Parse(hInputField.text);
            }
            catch
            {
                Debug.LogError("[GUI/ColorPicker] H input field is not a number!");
                hInputField.text = 0.ToString();
                return 0;
            }
        }
        set { hInputField.text = value.ToString(); hSlider.value = value; }
    }

    int s
    {
        get
        {
            try
            {
                return int.Parse(sInputField.text);
            }
            catch
            {
                Debug.LogError("[GUI/ColorPicker] S input field is not a number!");
                sInputField.text = 0.ToString();
                return 0;
            }
        }
        set { sInputField.text = value.ToString(); sSlider.value = value; }
    }

    int l
    {
        get
        {
            try
            {
                return int.Parse(lInputField.text);
            }
            catch
            {
                Debug.LogError("[GUI/ColorPicker] L input field is not a number!");
                lInputField.text = 0.ToString();
                return 0;
            }
        }
        set { lInputField.text = value.ToString(); lSlider.value = value; }
    }

    float hDec { get { return (float) h / 255; } }
    float sDec { get { return (float) s / 255; } }
    float lDec { get { return (float) l / 255; } }

    public Slider hSlider;
    public Slider sSlider;
    public Slider lSlider;

    public InputField hInputField;
    public InputField sInputField;
    public InputField lInputField;


    [HideInInspector]
    public MonoBehaviour callbackScript;
    [HideInInspector]
    public string callbackMethod;

    void Start()
    {
        if (hSlider == null || sSlider == null || lSlider == null)
            Debug.LogError("[GUI\\C0olorPicker] missing a slider object.");
        if (hInputField == null || sInputField == null || lInputField == null)
            Debug.LogError("[GUI\\C0olorPicker] missing an input field object.");
    }

    public void OpenColorPicker(MonoBehaviour _callbackScript, string _callbackMethod)
    {
        callbackScript = _callbackScript;
        callbackMethod = _callbackMethod;
        gameObject.SetActive(true);

    }

    public void OpenColorPicker(MonoBehaviour _callbackScript, string _callbackMethod, Color _currentColor)
    {
        callbackScript = _callbackScript;
        callbackMethod = _callbackMethod;
        gameObject.SetActive(true);

        SetSliders(_currentColor);
    }

    void SetSliders(Color _color)
    {
        float _h;
        float _s;
        float _l;

        Color.RGBToHSV(_color, out _h, out _s, out _l);

        hSlider.value = _h * 255;
        sSlider.value = _s * 255;
        lSlider.value = _l * 255;
    }

    void ResetValues()
    {
        hSlider.value = 0;
        sSlider.value = 255;
        lSlider.value = 255;
    }

    public void UpdateSliders()
    {
        h = (int) hSlider.value;
        s = (int) sSlider.value;
        l = (int) lSlider.value;

        UpdatePreviews();
    }

    public void UpdateInputFields()
    {
        hSlider.value = h;
        sSlider.value = s;
        lSlider.value = l;

        UpdatePreviews();
    }

    public void UpdatePreviews()
    {
        transform.FindChild("Background").FindChild("S").FindChild("STint").GetComponent<Image>().color = Color.HSVToRGB(hDec, 1, 1);
        transform.FindChild("Background").FindChild("Preview").GetComponent<Image>().color = Color.HSVToRGB(hDec, sDec, lDec);
    }

    public void Done()
    {
        gameObject.SetActive(false);
        callbackScript.StartCoroutine(callbackMethod, Color.HSVToRGB(hDec,sDec,lDec));
    }

}
