using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPicker : MonoBehaviour {

    float h;
    float s;
    float l;

    public Slider hSlider;
    public Slider sSlider;
    public Slider lSlider;

    public MonoBehaviour callbackScript;
    public string callbackMethod;

    void Start()
    {
        if (hSlider == null || sSlider == null || lSlider == null)
            Debug.LogError("[GUI\\C0olorPicker] missing a slider object.");
    }

    public void OpenColorPicker(MonoBehaviour _callbackScript, string _callbackMethod)
    {
        callbackScript = _callbackScript;
        callbackMethod = _callbackMethod;
        gameObject.SetActive(true);

        //ResetSliders();
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

        hSlider.value = _h;
        sSlider.value = _s;
        lSlider.value = _l;
    }

    void ResetSliders()
    {
        hSlider.value = 0;
        sSlider.value = 1;
        lSlider.value = 1;
    }

    public void UpdateSliders()
    {
        h = hSlider.value;
        s = sSlider.value;
        l = lSlider.value;

        UpdatePreviews();
    }

    public void UpdatePreviews()
    {
        transform.FindChild("Background").FindChild("S").FindChild("STint").GetComponent<Image>().color = Color.HSVToRGB(h, 1, 1);
        transform.FindChild("Background").FindChild("Preview").GetComponent<Image>().color = Color.HSVToRGB(h, s, l);
    }

    public void Done()
    {
        gameObject.SetActive(false);
        callbackScript.StartCoroutine(callbackMethod, Color.HSVToRGB(h,s, l));
    }

}
