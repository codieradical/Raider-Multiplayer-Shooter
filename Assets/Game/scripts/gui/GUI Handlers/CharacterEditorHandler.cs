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

    public Image primaryButton;
    public Image secondaryButton;
    public Image tertiaryButton;
    public Text usernameLabel;

    private GameObject modelTorso;
    private GameObject modelLimbs;
    private GameObject modelJoints;

    void Start()
    {
        if (primaryButton == null || secondaryButton == null || tertiaryButton == null || usernameLabel == null)
            Debug.LogError("[GUI/CharacterEditorHandler] Missing a required game object.");
    }

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

        usernameLabel.text = Session.saveDataHandler.GetUsername();

        modelTorso = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighTorsoGeo").gameObject;
        modelLimbs = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighLimbsGeo").gameObject;
        modelJoints = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighJointsGeo").gameObject;
    }

    public void UpdateColor(Color color, int index)
    {
        if (index == 1)
        {
            primarycolor = color;
            modelTorso.GetComponent<Renderer>().material.color = primarycolor;
            primaryButton.color = primarycolor;
        }
        else if (index == 2)
        {
            secondarycolor = color;
            modelJoints.GetComponent<Renderer>().material.color = secondarycolor;
            secondaryButton.color = secondarycolor;
        }
        else if (index == 3)
        {
            tertiarycolor = color;
            modelLimbs.GetComponent<Renderer>().material.color = tertiarycolor;
            tertiaryButton.color = tertiarycolor;
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
            colorPicker.GetComponent<ColorPicker>().OpenColorPicker(this, "SetColor1", primaryButton.color);
        }
        else if (index == 2)
        {
            colorPicker.GetComponent<ColorPicker>().OpenColorPicker(this, "SetColor2", secondaryButton.color);
        }
        else if (index == 3)
        {
            colorPicker.GetComponent<ColorPicker>().OpenColorPicker(this, "SetColor3", tertiaryButton.color);
        }
        else
        {
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for EditColor method.");
        }
    }

}
