using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class CharacterEditorHandler : MonoBehaviour {

    [Header("Characters")]
    public GameObject charactersParent;

    [Header("Prefabs")]
    public Object selectedCharacterPrefab;

    //Assigned by mainmenu handler.
    public GameObject colorPicker;

    private RenderTexture selectedCharacterView;
    private GameObject selectedCharacterDisplayModel;
    private Color primarycolor;
    private Color secondarycolor;
    private Color tertiarycolor;

    private GameObject modelTorso;
    private GameObject modelLimbs;
    private GameObject modelJoints;

    public void NewCharacter()
    {
        GameObject newCharacter = Instantiate(selectedCharacterPrefab) as GameObject;
        newCharacter.transform.SetParent(charactersParent.transform, false);
        newCharacter.name = "SelectedChar";

        selectedCharacterDisplayModel = newCharacter;

        selectedCharacterView = new RenderTexture(512, 515, 24, RenderTextureFormat.ARGB32);
        selectedCharacterView.Create();

        charactersParent.transform.FindChild("SelectedChar").FindChild("cam").GetComponent<Camera>().targetTexture = selectedCharacterView;
        transform.FindChild("Image").GetComponent<RawImage>().texture = selectedCharacterView;

        modelTorso = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighTorsoGeo").gameObject;
        modelLimbs = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighLimbsGeo").gameObject;
        modelJoints = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighJointsGeo").gameObject;
    }

    public void UpdateColor(Color color, int index)
    {
        if (index == 1)
        {
            primarycolor = color;
            modelTorso.GetComponent<Renderer>().material.SetColor(1, primarycolor);
        }
        else if (index == 2)
        {
            secondarycolor = color;
            modelJoints.GetComponent<Renderer>().material.SetColor(1, secondarycolor);
        }
        else if (index == 3)
        {
            tertiarycolor = color;
            modelLimbs.GetComponent<Renderer>().material.SetColor(1, tertiarycolor);
        }
        else
        {
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for UpdateColor method.");
        }
    }

    public void SetColor1(Color color)
    {
        UpdateColor(color, 1);
    }

    public void SetColor2(Color color)
    {
        UpdateColor(color, 2);
    }

    public void SetColor3(Color color)
    {
        UpdateColor(color, 3);
    }

    public void EditColor(int index)
    {
        if(index == 1)
        {
            colorPicker.GetComponent<ColorPicker>().OpenColorPicker(this, "SetColor1");
        }
        else if (index == 2)
        {
            colorPicker.GetComponent<ColorPicker>().OpenColorPicker(this, "SetColor2");
        }
        else if (index == 3)
        {
            colorPicker.GetComponent<ColorPicker>().OpenColorPicker(this, "SetColor3");
        }
        else
        {
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for EditColor method.");
        }
    }

}
