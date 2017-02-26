using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Globalization;
using System;
using Raider.Game.Saves;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Saves.User;

namespace Raider.Game.GUI.Screens
{
    //This class could use a little refactoring.
    //An Update Form method should be added, taking some function from randomize and update previews.
    public class CharacterEditorHandler : MonoBehaviour
    {
        [Header("Objects")]

        public EmblemEditorHandler emblemEditor;
        public EmblemHandler emblemPreview;

        public Text titleText;
        public Dropdown raceDropdown;
        public Image primaryColorButton;
        public Image secondaryColorButton;
        public Image tertiaryColorButton;
        public Text usernameLabel;
        public InputField guildInput;

        public GameObject characterPreviewImage;
        RawImage characterPreviewRawImage;

        const string PREVIEW_CHARACTER_NAME = "EditingChar";
        const CharacterPreviewHandler.PreviewType PREVIEW_TYPE = CharacterPreviewHandler.PreviewType.Full;

        public UserSaveDataStructure.Character editingCharacter;

        [HideInInspector]
        public int characterSlot;

        #region setup

        void Start()
        {
            characterPreviewRawImage = characterPreviewImage.GetComponent<RawImage>();

            emblemEditor.characterEditorHandler = this;

            if (primaryColorButton == null || secondaryColorButton == null || tertiaryColorButton == null || usernameLabel == null)
                Debug.LogError("[GUI/CharacterEditorHandler] Missing a required game object.");
        }

        public void NewCharacter()
        {
            /*Setup the GUI*/
            titleText.text = "Create a Character";
            usernameLabel.text = Session.userSaveDataHandler.GetUsername();

            /*Setup the Save Data Handler*/
            characterSlot = Session.userSaveDataHandler.CharacterCount;
            editingCharacter = new UserSaveDataStructure.Character();

            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());

            /*Setup the UI data*/
            ResetFormValues();

            RandomiseCharacter();
            RandomiseEmblem();

            UpdateFormValues();
            UpdatePreview();
        }

        public void EditCharacter(int _slot)
        {
            titleText.text = "Edit a Character";
            usernameLabel.text = Session.userSaveDataHandler.GetUsername();

            characterSlot = _slot;
            editingCharacter = Session.userSaveDataHandler.GetCharacter(_slot);

            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());

            ResetFormValues();
            UpdateFormValues();
            UpdatePreview();
        }

        public void UpdatePreview()
        {
            CharacterPreviewHandler.instance.EnqueuePreviewUpdate(PREVIEW_CHARACTER_NAME, editingCharacter);

            emblemPreview.UpdateEmblem(editingCharacter);
        }

        #endregion

        #region data

        public void ResetFormValues()
        {
            raceDropdown.options = new List<Dropdown.OptionData>();
            foreach (UserSaveDataStructure.Character.Races race in Enum.GetValues(typeof(UserSaveDataStructure.Character.Races)))
            {
                raceDropdown.options.Add(new Dropdown.OptionData(race.ToString()));
            }
            raceDropdown.value = (int)editingCharacter.Race;
            guildInput.text = editingCharacter.guild;
        }

        public void UpdateFormValues()
        {
            raceDropdown.value = (int)editingCharacter.Race; //This calls the onchange on the dropdown btw.
            guildInput.text = editingCharacter.guild; //This calls edit guild, so make sure tha edit guild doesn't call this.
            primaryColorButton.color = editingCharacter.armourPrimaryColor.Color;
            secondaryColorButton.color = editingCharacter.armourSecondaryColor.Color;
            tertiaryColorButton.color = editingCharacter.armourTertiaryColor.Color;
        }


        public void EditGuild(string _guild)
        {
            //This is called by the form, so nothing needs updating.
            editingCharacter.guild = _guild;
        }

        /// <summary>
        /// Updates the model races and previews when the race dropdown is used.
        /// </summary>
        /// <param name="newRace">The int value provided by the form, cast to a race enum value.</param>
        public void EditRace(int newRace)
        {
            editingCharacter.Race = (UserSaveDataStructure.Character.Races)newRace;

            try {
                CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);
            } catch { }

            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());
        }

        public void RandomiseCharacter()
        {
            editingCharacter.armourPrimaryColor = new CommonSaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.armourSecondaryColor = new CommonSaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.armourTertiaryColor = new CommonSaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());

            //unity rand isn't good at certain things...

            System.Random rand = new System.Random();

            //Creates a random number between 0 and the amount of races available, parses that as a race.
            editingCharacter.Race = (UserSaveDataStructure.Character.Races)rand.Next(0, Enum.GetNames(typeof(UserSaveDataStructure.Character.Races)).Length);

            UpdateFormValues();
            UpdatePreview(); //Update the preview to make sure that the buttons are up to date.
        }

        public void RandomiseEmblem()
        {
			editingCharacter.emblem.layer0 = UnityEngine.Random.Range(0, EmblemHandler.layer0sprites.Length - 1);
			editingCharacter.emblem.layer1 = UnityEngine.Random.Range(0, EmblemHandler.layer1sprites.Length - 1);
            System.Random rand = new System.Random();
            editingCharacter.emblem.layer2 = Convert.ToBoolean(rand.Next(0, 2));

            editingCharacter.emblem.layer0Color = new CommonSaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.emblem.layer1Color =  new CommonSaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.emblem.layer2Color =  new CommonSaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());

            UpdatePreview();
        }

        #endregion

        #region color stuff

        public void UpdateColor(Color color, int index)
        {
            if (index == 1)
                editingCharacter.armourPrimaryColor = new CommonSaveDataStructure.SerializableColor(color);
            else if (index == 2)
                editingCharacter.armourSecondaryColor = new CommonSaveDataStructure.SerializableColor(color);
            else if (index == 3)
                editingCharacter.armourTertiaryColor = new CommonSaveDataStructure.SerializableColor(color);
            else
                Debug.Log("[GUI\\CharacterEditor] Invalid index provided for UpdateColor method.");

            UpdateFormValues();
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
            if (index == 1)
                HSLColorPicker.instance.OpenColorPicker(SetColor1, primaryColorButton.color);
            else if (index == 2)
                HSLColorPicker.instance.OpenColorPicker(SetColor2, secondaryColorButton.color);
            else if (index == 3)
                HSLColorPicker.instance.OpenColorPicker(SetColor3, tertiaryColorButton.color);
            else
                Debug.Log("[GUI\\CharacterEditor] Invalid index provided for EditColor method.");
        }

        #endregion

        public void Done()
        {
            if (characterSlot == Session.userSaveDataHandler.CharacterCount)
                Session.userSaveDataHandler.NewCharacter(editingCharacter, null, EditCharacterSaveFailure);
            else
                Session.userSaveDataHandler.SaveCharacter(characterSlot, editingCharacter, null, EditCharacterSaveFailure);

            //Delete the preview.
            CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);

            //Update active screen.
            MainmenuController.instance.CloseCharacterEditor();

            //Dispose of any unneeded values
            editingCharacter = null;
        }

        private void EditCharacterSaveFailure(string error)
        {
            UserFeedback.LogError(error);
        }
    }
}