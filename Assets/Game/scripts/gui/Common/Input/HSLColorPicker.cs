using System;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{

	public class HSLColorPicker : MonoBehaviour
    {

        #region Singleton Setup
        //Methods could possibly be refactored a little.

        public static HSLColorPicker instance;

        //constructor needs to be used, as this componenent is awoken by instance reference.
        HSLColorPicker()
        {
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        int H
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

        int S
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

        int L
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

        float HDec { get { return (float)H / 255; } }
        float SDec { get { return (float)S / 255; } }
        float LDec { get { return (float)L / 255; } }

        public Slider hSlider;
        public Slider sSlider;
        public Slider lSlider;

        public InputField hInputField;
        public InputField sInputField;
        public InputField lInputField;


        [HideInInspector]
        public Action<Color> callbackMethod;

        void Start()
        {
            if (hSlider == null || sSlider == null || lSlider == null)
                Debug.LogError("[GUI\\C0olorPicker] missing a slider object.");
            if (hInputField == null || sInputField == null || lInputField == null)
                Debug.LogError("[GUI\\C0olorPicker] missing an input field object.");
        }

        public void OpenColorPicker(Action<Color> callback)
        {
            callbackMethod = callback;
            gameObject.SetActive(true);
        }

        public void OpenColorPicker(Action<Color> callback, Color currentColor)
        {
            OpenColorPicker(callback);
            SetSliders(currentColor);
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
            H = (int)hSlider.value;
            S = (int)sSlider.value;
            L = (int)lSlider.value;

            UpdatePreviews();
        }

        public void UpdateInputFields()
        {
            hSlider.value = H;
            sSlider.value = S;
            lSlider.value = L;

            UpdatePreviews();
        }

        public void UpdatePreviews()
        {
            transform.Find("Background").Find("S").Find("STint").GetComponent<Image>().color = Color.HSVToRGB(HDec, 1, 1);
            transform.Find("Background").Find("Preview").GetComponent<Image>().color = Color.HSVToRGB(HDec, SDec, LDec);
        }

        public void Done()
        {
            gameObject.SetActive(false);
            callbackMethod(Color.HSVToRGB(HDec, SDec, LDec));
        }
    }
}