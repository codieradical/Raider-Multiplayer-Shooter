using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Globalization;
using System;
using Raider.Game.Saves;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.CharacterPreviews;

namespace Raider.Game.GUI.Screens
{
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

        public SaveDataStructure.Character editingCharacter;

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
            titleText.text = "Create a Character";
            characterSlot = Session.saveDataHandler.characterCount;
            editingCharacter = new SaveDataStructure.Character();

            RandomiseCharacter();
            RandomiseEmblem();

            ResetFieldValues();
        }

        public void EditCharacter(int _slot)
        {
            titleText.text = "Edit a Character";
            characterSlot = _slot;
            editingCharacter = Session.saveDataHandler.GetCharacter(_slot);

            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());

            ResetFieldValues();
            UpdatePreview();
        }

        public void UpdatePreview()
        {
            CharacterPreviewHandler.instance.EnqueuePreviewUpdate(PREVIEW_CHARACTER_NAME, editingCharacter);

            primaryColorButton.color = editingCharacter.armourPrimaryColor.color;
            secondaryColorButton.color = editingCharacter.armourSecondaryColor.color;
            tertiaryColorButton.color = editingCharacter.armourTertiaryColor.color;

            emblemPreview.UpdateEmblem(editingCharacter);
        }

        #endregion

        #region data

        public void ResetFieldValues()
        {
            raceDropdown.options = new List<Dropdown.OptionData>();
            foreach (SaveDataStructure.Character.Race race in Enum.GetValues(typeof(SaveDataStructure.Character.Race)))
            {
                raceDropdown.options.Add(new Dropdown.OptionData(race.ToString()));
            }
            raceDropdown.value = (int)editingCharacter.race;

            usernameLabel.text = Session.saveDataHandler.GetUsername();
            guildInput.text = editingCharacter.guild;
        }

        public void EditGuild(string _guild)
        {
            editingCharacter.guild = _guild;
        }

        public void EditRace(int _raceValue)
        {
            editingCharacter.race = (SaveDataStructure.Character.Race)_raceValue;

            try {
                CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);
            } catch { }
            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, editingCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());
        }

        public void RandomiseCharacter()
        {
            editingCharacter.armourPrimaryColor = new SaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.armourSecondaryColor = new SaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.armourTertiaryColor = new SaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());

            //unity rand isn't good at certain things...

            System.Random rand = new System.Random();

            SaveDataStructure.Character.Race newRace = (SaveDataStructure.Character.Race)rand.Next(0, Enum.GetNames(typeof(SaveDataStructure.Character.Race)).Length);

            EditRace((int)newRace);
            UpdatePreview(); //Update the preview to make sure that the buttons are up to date.
        }

        public void RandomiseEmblem()
        {
            editingCharacter.emblemLayer0 = UnityEngine.Random.Range(0, emblemPreview.layer0sprites.Length - 1);
            editingCharacter.emblemLayer1 = UnityEngine.Random.Range(0, emblemPreview.layer1sprites.Length - 1);
            System.Random rand = new System.Random();
            editingCharacter.emblemLayer2 = Convert.ToBoolean(rand.Next(0, 2));

            editingCharacter.emblemLayer0Color = new SaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.emblemLayer1Color =  new SaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());
            editingCharacter.emblemLayer2Color =  new SaveDataStructure.SerializableColor(UnityEngine.Random.ColorHSV());

            UpdatePreview();
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
            if (index == 1)
                HSLColorPicker.instance.OpenColorPicker(this, "SetColor1", primaryColorButton.color);
            else if (index == 2)
                HSLColorPicker.instance.OpenColorPicker(this, "SetColor2", secondaryColorButton.color);
            else if (index == 3)
                HSLColorPicker.instance.OpenColorPicker(this, "SetColor3", tertiaryColorButton.color);
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
            CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);

            //Update active screen.
            MainmenuHandler.instance.CloseCharacterEditor();

            //Dispose of any unneeded values
            editingCharacter = null;
        }
    }
}