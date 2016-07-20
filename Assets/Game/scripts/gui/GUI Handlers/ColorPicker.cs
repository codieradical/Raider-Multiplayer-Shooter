using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPicker : MonoBehaviour {

    float h;
    float s;
    float l;

    public MonoBehaviour callbackScript;
    public string callbackMethod;

    public void OpenColorPicker(MonoBehaviour _callbackScript, string _callbackMethod)
    {
        callbackScript = _callbackScript;
        callbackMethod = _callbackMethod;
        gameObject.SetActive(true);
    }

    public void UpdateSliders()
    {
        h = transform.FindChild("Background").FindChild("H").FindChild("HSlider").GetComponent<Slider>().value;
        s = transform.FindChild("Background").FindChild("S").FindChild("SSlider").GetComponent<Slider>().value;
        l = transform.FindChild("Background").FindChild("L").FindChild("LSlider").GetComponent<Slider>().value;

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
