using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{
    [RequireComponent(typeof(Animator))]
    public class OptionsPaneHandler : MonoBehaviour
    {

        #region Singleton Setup

        public static OptionsPaneHandler instance;
        Animator animatorInstance;

        void Start()
        {
            if (instance == null)
                instance = this;
            else
                Debug.Log("Multiple OptionsPaneHandler Instances");

            animatorInstance = GetComponent<Animator>();
        }

        void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public List<OptionsPaneOption> options = new List<OptionsPaneOption>();

        public Text paneTitle;
        public GameObject paneSecondary;
        public GameObject paneTertiary;

        public UnityEngine.Object paneSecondaryPrefab;
        public UnityEngine.Object paneTertiaryPrefab;

        public Action voidCallback;
        public Action<string> stringCallback;

        public void ShowOptions(string _paneTitle, List<OptionsPaneOption.OptionsPaneContents> _optionsContents, Action callback)
        {
            paneTitle.text = _paneTitle;
            voidCallback = callback;
            SetupOptionsObjects(_optionsContents);
            animatorInstance.SetBool("open", true);
        }

        public void ShowOptions(string _paneTitle, List<OptionsPaneOption.OptionsPaneContents> _optionsContents, Action<string> callback)
        {
            paneTitle.text = _paneTitle;
            stringCallback = callback;
            SetupOptionsObjects(_optionsContents);
            animatorInstance.SetBool("open", true);
        }

        void SetupOptionsObjects(List<OptionsPaneOption.OptionsPaneContents> optionsData)
        {
            foreach (OptionsPaneOption.OptionsPaneContents optionData in optionsData)
            {
                GameObject newOption = Instantiate(paneSecondaryPrefab) as GameObject;
                newOption.GetComponent<OptionsPaneOption>().optionData = optionData;
                newOption.transform.SetParent(paneSecondary.transform);
            }
        }

        public void OptionClicked(string optionName)
        {
            if (stringCallback != null)
                stringCallback(optionName);
            else
                voidCallback();
            HideOptions();
            ResetData();
        }

        public void HideOptions()
        {
            animatorInstance.SetBool("open", false);
        }

        void ResetData()
        {
            paneTitle.text = "OPTIONS_PANE";
            foreach (Transform child in paneSecondary.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in paneTertiary.transform)
            {
                Destroy(child.gameObject);
            }

            voidCallback = null;
            stringCallback = null;
            options = new List<OptionsPaneOption>();
        }

        public void OptionHover(OptionsPaneOption.OptionsPaneContents hoveredOption)
        {
            GameObject newDescription = Instantiate(paneTertiaryPrefab) as GameObject;

            newDescription.transform.SetParent(paneTertiary.transform, false);

            //If there's an image available, show it.
            //Else, set the image scale to 0.
            //It's important to keep the image, as the description is anchored to the bottom.
            if (hoveredOption.image != null)
                newDescription.transform.Find("Image").GetComponent<Image>().sprite = hoveredOption.image;
            else
            {
                RectTransform imageRect = newDescription.transform.Find("Image").GetComponent<RectTransform>();
                Vector2 newSize = new Vector3(imageRect.sizeDelta.x, 0);
                imageRect.sizeDelta = newSize;
            }
            newDescription.transform.Find("Image").Find("Text").GetComponent<Text>().text = hoveredOption.description;
        }

        public void OptionStopHover()
        {
            //When the cursor is not hovering an option, destroy the description in pane 3.
            foreach (Transform child in paneTertiary.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}