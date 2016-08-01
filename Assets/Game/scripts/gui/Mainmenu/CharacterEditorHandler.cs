using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class CharacterEditorHandler : MonoBehaviour {

    [Header("Objects")]
    [HideInInspector]    //Assigned by mainmenu handler.
    public ColorPicker colorPicker;
    public EmblemEditorHandler emblemEditor;
    public EmblemHandler emblemPreview;
    public CharacterPreviewHandler characterPreviewHandler;

    const string previewCharacterName = "SelectedChar";

    private RenderTexture previewCharacterImage;

    public SaveDataStructure.Character character;

    public RawImage characterPreviewImage;
    public Image primaryColorButton;
    public Image secondaryColorButton;
    public Image tertiaryColorButton;
    public Text usernameLabel;

    void Start()
    {
        emblemEditor.characterEditorHandler = this;

        if (primaryColorButton == null || secondaryColorButton == null || tertiaryColorButton == null || usernameLabel == null)
            Debug.LogError("[GUI/CharacterEditorHandler] Missing a required game object.");
    }

    public void NewCharacter()
    {
        character = new SaveDataStructure.Character();

        Camera previewCamera;

        characterPreviewHandler.NewPreview("SelectedChar", character, out previewCamera);

        previewCharacterImage = new RenderTexture(512, 515, 24, RenderTextureFormat.ARGB32);
        previewCharacterImage.Create();

        previewCamera.targetTexture = previewCharacterImage;
        transform.FindChild("Image").GetComponent<RawImage>().texture = previewCharacterImage;

        usernameLabel.text = Session.saveDataHandler.GetUsername();

        UpdatePreview();
    }

    public void UpdatePreview()
    {
        characterPreviewHandler.PushPreviewUpdate("SelectedChar", character);

        primaryColorButton.color = character.armourPrimaryColor;
        secondaryColorButton.color = character.armourSecondaryColor;
        tertiaryColorButton.color = character.armourTertiaryColor;

        emblemPreview.UpdateEmblem(character);
    }

    public void UpdateColor(Color color, int index)
    {
        if (index == 1)
            character.armourPrimaryColor = color;
        else if (index == 2)
            character.armourSecondaryColor = color;
        else if (index == 3)
            character.armourTertiaryColor = color;
        else
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for UpdateColor method.");

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
            colorPicker.OpenColorPicker(this, "SetColor1", primaryColorButton.color);
        else if (index == 2)
            colorPicker.OpenColorPicker(this, "SetColor2", secondaryColorButton.color);
        else if (index == 3)
            colorPicker.OpenColorPicker(this, "SetColor3", tertiaryColorButton.color);
        else
            Debug.Log("[GUI\\CharacterEditor] Invalid index provided for EditColor method.");
    }

}
