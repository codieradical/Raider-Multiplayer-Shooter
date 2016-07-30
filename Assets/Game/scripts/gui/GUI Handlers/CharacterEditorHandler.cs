using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class CharacterEditorHandler : MonoBehaviour {

    [Header("Objects")]
    public GameObject charactersParent;
    [HideInInspector]    //Assigned by mainmenu handler.
    public ColorPicker colorPicker;
    public EmblemEditorHandler emblemEditor;
    public EmblemHandler emblemPreview;

    [Header("Prefabs")]
    public Object selectedCharacterPrefab;

    private RenderTexture selectedCharacterView;
    private GameObject selectedCharacterDisplayModel;

    public SaveDataStructure.Character character;

    public Image primaryButton;
    public Image secondaryButton;
    public Image tertiaryButton;
    public Text usernameLabel;

    private GameObject modelTorso;
    private GameObject modelLimbs;
    private GameObject modelJoints;

    void Start()
    {
        emblemEditor.characterEditorHandler = this;

        if (primaryButton == null || secondaryButton == null || tertiaryButton == null || usernameLabel == null)
            Debug.LogError("[GUI/CharacterEditorHandler] Missing a required game object.");
    }

    public void NewCharacter()
    {
        character = new SaveDataStructure.Character();

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

        UpdatePreview();
    }

    public void UpdatePreview()
    {
        modelTorso.GetComponent<Renderer>().material.color = character.armourPrimaryColor;
        primaryButton.color = character.armourPrimaryColor;
        modelJoints.GetComponent<Renderer>().material.color = character.armourSecondaryColor;
        secondaryButton.color = character.armourSecondaryColor;
        modelLimbs.GetComponent<Renderer>().material.color = character.armourTertiaryColor;
        tertiaryButton.color = character.armourTertiaryColor;

        emblemPreview.UpdateEmblem(character);
    }

    public void UpdateColor(Color color, int index)
    {
        if (index == 1)
        {
            character.armourPrimaryColor = color;
        }
        else if (index == 2)
        {
            character.armourSecondaryColor = color;
        }
        else if (index == 3)
        {
            character.armourTertiaryColor = color;
        }
        else
        {
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for UpdateColor method.");
        }

        UpdatePreview();
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
            colorPicker.OpenColorPicker(this, "SetColor1", primaryButton.color);
        }
        else if (index == 2)
        {
            colorPicker.OpenColorPicker(this, "SetColor2", secondaryButton.color);
        }
        else if (index == 3)
        {
            colorPicker.OpenColorPicker(this, "SetColor3", tertiaryButton.color);
        }
        else
        {
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for EditColor method.");
        }
    }

}
