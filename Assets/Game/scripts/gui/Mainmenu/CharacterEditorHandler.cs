using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class CharacterEditorHandler : MonoBehaviour {

    [Header("Objects")]
    [HideInInspector]    //Assigned by mainmenu handler.
    public HSLColorPicker colorPicker;
    [HideInInspector]
    public CharacterPreviewHandler characterPreviewHandler;
    [HideInInspector]
    public MainmenuHandler mainmenuHandler;

    public EmblemEditorHandler emblemEditor;
    public EmblemHandler emblemPreview;
    public Image primaryColorButton;
    public Image secondaryColorButton;
    public Image tertiaryColorButton;
    public Text usernameLabel;
    public GameObject characterPreviewImage;

    const string PREVIEW_CHARACTER_NAME = "EditingChar";

    private RenderTexture previewCharacterImage;

    public SaveDataStructure.Character editingCharacter;

    public int characterSlot;


    void Start()
    {
        emblemEditor.characterEditorHandler = this;

        if (primaryColorButton == null || secondaryColorButton == null || tertiaryColorButton == null || usernameLabel == null)
            Debug.LogError("[GUI/CharacterEditorHandler] Missing a required game object.");
    }

    public void NewCharacter()
    {
        characterSlot = Session.saveDataHandler.characterCount;
        editingCharacter = new SaveDataStructure.Character();

        SetupPreviewImage(editingCharacter);

        usernameLabel.text = Session.saveDataHandler.GetUsername();

        UpdatePreview();
    }

    public void EditCharacter(int _slot)
    {
        characterSlot = _slot;
        editingCharacter = Session.saveDataHandler.GetCharacter(_slot);

        SetupPreviewImage(editingCharacter);
    }

    void SetupPreviewImage(SaveDataStructure.Character _editingCharacter)
    {
        Camera previewCamera;
        CharacterPreviewDisplayHandler displayHandler = characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>();

        characterPreviewHandler.NewPreview(PREVIEW_CHARACTER_NAME, _editingCharacter, CharacterPreviewHandler.PreviewType.Full, out previewCamera, displayHandler);

        previewCharacterImage = new RenderTexture(Screen.height, Screen.height, 24, RenderTextureFormat.ARGB32);
        previewCharacterImage.Create();

        previewCamera.targetTexture = previewCharacterImage;
        characterPreviewImage.GetComponent<RawImage>().texture = previewCharacterImage;
    }

    void SetupPreviewImage()
    {
        Camera previewCamera = characterPreviewHandler.GetPreviewCamera(PREVIEW_CHARACTER_NAME);

        previewCharacterImage = new RenderTexture(Screen.height, Screen.height, 24, RenderTextureFormat.ARGB32);
        previewCharacterImage.Create();

        previewCamera.targetTexture = previewCharacterImage;
        characterPreviewImage.GetComponent<RawImage>().texture = previewCharacterImage;
    }

    public void UpdatePreview()
    {
        bool newPreview;
        characterPreviewHandler.PushPreviewUpdate(PREVIEW_CHARACTER_NAME, editingCharacter, out newPreview);
        if (newPreview)
            SetupPreviewImage();

        primaryColorButton.color = editingCharacter.armourPrimaryColor.color;
        secondaryColorButton.color = editingCharacter.armourSecondaryColor.color;
        tertiaryColorButton.color = editingCharacter.armourTertiaryColor.color;

        emblemPreview.UpdateEmblem(editingCharacter);
    }

    public void UpdateColor(Color color, int index)
    {
        if (index == 1)
            editingCharacter.armourPrimaryColor = new SaveDataStructure.SerializableColor(color);
        else if (index == 2)
            editingCharacter.armourSecondaryColor = new SaveDataStructure.SerializableColor(color);
        else if (index == 3)
            editingCharacter.armourTertiaryColor = new SaveDataStructure.SerializableColor(color);
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

    public void Done()
    {
        if (characterSlot == Session.saveDataHandler.characterCount)
            Session.saveDataHandler.NewCharacter(editingCharacter);
        else
            Session.saveDataHandler.SaveCharacter(characterSlot, editingCharacter);

        //Delete the preview.
        Destroy(characterPreviewHandler.GetPreviewObject(PREVIEW_CHARACTER_NAME));

        //Update active screen.
        mainmenuHandler.CloseCharacterEditor();

        //Dispose of any unneeded values
        editingCharacter = null;
    }
}
