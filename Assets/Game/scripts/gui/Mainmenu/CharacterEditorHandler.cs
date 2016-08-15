using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;
using System;

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

    public GameObject optionsParent;
    private Dropdown raceDropdown;
    private Image primaryColorButton;
    private Image secondaryColorButton;
    private Image tertiaryColorButton;
    private Text usernameLabel;
    private InputField guildInput;

    public GameObject characterPreviewImage;
    RawImage characterPreviewRawImage;

    const string PREVIEW_CHARACTER_NAME = "EditingChar";
    const CharacterPreviewHandler.PreviewType PREVIEW_TYPE = CharacterPreviewHandler.PreviewType.Full;

    private RenderTexture previewCharacterImage;

    public SaveDataStructure.Character editingCharacter;

    [HideInInspector]
    public int characterSlot;

    #region setup

    void Start()
    {
        characterPreviewRawImage = characterPreviewImage.GetComponent<RawImage>();

        emblemEditor.characterEditorHandler = this;

        FindOptionFields();

        if (primaryColorButton == null || secondaryColorButton == null || tertiaryColorButton == null || usernameLabel == null)
            Debug.LogError("[GUI/CharacterEditorHandler] Missing a required game object.");
    }

    public void FindOptionFields()
    {
        //A better way to assign these would be nice.
        primaryColorButton = optionsParent.transform.Find("color1").Find("Button").GetComponent<Image>();
        secondaryColorButton = optionsParent.transform.Find("color2").Find("Button").GetComponent<Image>();
        tertiaryColorButton = optionsParent.transform.Find("color3").Find("Button").GetComponent<Image>();
        usernameLabel = optionsParent.transform.Find("username").Find("label").GetComponent<Text>();
        guildInput = optionsParent.transform.Find("guild").Find("InputField").GetComponent<InputField>();
        raceDropdown = optionsParent.transform.Find("race").Find("Dropdown").GetComponent<Dropdown>();
    }

    public void NewCharacter()
    {
        characterSlot = Session.saveDataHandler.characterCount;
        editingCharacter = new SaveDataStructure.Character();

        characterPreviewHandler.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage);

        ResetFieldValues();
        UpdatePreview();
    }

    public void EditCharacter(int _slot)
    {
        characterSlot = _slot;
        editingCharacter = Session.saveDataHandler.GetCharacter(_slot);

        characterPreviewHandler.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage);

        ResetFieldValues();
        UpdatePreview();
    }

    public void UpdatePreview()
    {
        characterPreviewHandler.PushPreviewUpdate(PREVIEW_CHARACTER_NAME, editingCharacter);

        primaryColorButton.color = editingCharacter.armourPrimaryColor.color;
        secondaryColorButton.color = editingCharacter.armourSecondaryColor.color;
        tertiaryColorButton.color = editingCharacter.armourTertiaryColor.color;

        emblemPreview.UpdateEmblem(editingCharacter);
    }

    #endregion

    #region data

    public void ResetFieldValues()
    {
        raceDropdown.options = new System.Collections.Generic.List<Dropdown.OptionData>();
        foreach(SaveDataStructure.Character.Race race in Enum.GetValues(typeof(SaveDataStructure.Character.Race)))
        {
            raceDropdown.options.Add(new Dropdown.OptionData(race.ToString()));
        }
        raceDropdown.value = (int) editingCharacter.race;

        usernameLabel.text = Session.saveDataHandler.GetUsername();
        guildInput.text = editingCharacter.guild;
    }

    public void EditGuild(string _guild)
    {
        editingCharacter.guild = _guild;
    }

    public void EditRace(Int32 _raceValue)
    {
        editingCharacter.race = (SaveDataStructure.Character.Race)_raceValue;

        characterPreviewHandler.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);
        characterPreviewHandler.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage);
    }

    #endregion

    #region color stuff

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

    #endregion

    public void Done()
    {
        if (characterSlot == Session.saveDataHandler.characterCount)
            Session.saveDataHandler.NewCharacter(editingCharacter);
        else
            Session.saveDataHandler.SaveCharacter(characterSlot, editingCharacter);

        //Delete the preview.
        characterPreviewHandler.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);

        //Update active screen.
        mainmenuHandler.CloseCharacterEditor();

        //Dispose of any unneeded values
        editingCharacter = null;
    }
}
