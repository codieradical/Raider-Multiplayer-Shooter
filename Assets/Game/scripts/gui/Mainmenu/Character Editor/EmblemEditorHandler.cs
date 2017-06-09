using Raider.Game.GUI.Components;
using Raider.Game.Saves;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
	public class EmblemEditorHandler : EmblemHandler
    {

        [HideInInspector]
        public CharacterEditorHandler characterEditorHandler; //Assigned by CharacterEditorHandler.

        [Header("Input")]
        public Toggle layer2field;
        public NumberField layer1field;
        public NumberField layer0field;

        public Image primaryButton;
        public Image secondaryButton;
        public Image tertiaryButton;

        public bool Layer2value { get { return layer2field.isOn; } }
        public int Layer1value { get { return layer1field.Value; } }
        public int Layer0value { get { return layer0field.Value; } }

        private Color layer0color;
        private Color layer1color;
        private Color layer2color;

        new void Awake()
        {
            //Make sure the character can't select an emblem index above the amount of emblems.
            base.Awake();
            layer1field.max = layer1sprites.Length - 1;
            layer0field.max = layer0sprites.Length - 1;
        }

        public void OpenEditor()
        {
            UpdateFields();
            UpdatePreview();
            MenuManager.instance.ShowMenu(this.GetComponent<Menu>());
        }

        public void CloseEditor()
        {
            MenuManager.instance.ShowMenu(characterEditorHandler.gameObject.GetComponent<Menu>());
        }

        void UpdateFields()
        {
            layer0color = characterEditorHandler.editingCharacter.emblem.layer0Color.Color;
            layer1color = characterEditorHandler.editingCharacter.emblem.layer1Color.Color;
            layer2color = characterEditorHandler.editingCharacter.emblem.layer2Color.Color;

            layer0field.Value = characterEditorHandler.editingCharacter.emblem.layer0;
            layer1field.Value = characterEditorHandler.editingCharacter.emblem.layer1;
            layer2field.isOn = characterEditorHandler.editingCharacter.emblem.layer2;
        }

        public void UpdatePreview()
        {
            //Update color buttons.
            primaryButton.color = layer0color;
            secondaryButton.color = layer1color;
            tertiaryButton.color = layer2color;

            //Update emblem color.
            layer0image.color = layer0color;
            layer1image.color = layer1color;
            layer2image.color = layer2color;

            //Update layer 2 toggle.
            layer2image.gameObject.SetActive(Layer2value);

            //Update layer images.
            layer0image.sprite = layer0sprites[Layer0value];
            layer1image.sprite = layer1sprites[Layer1value];
            layer2image.sprite = layer2sprites[Layer1value];
        }

        public void Done()
        {
            //update the emblem images
            characterEditorHandler.editingCharacter.emblem.layer0 = Layer0value;
            characterEditorHandler.editingCharacter.emblem.layer1 = Layer1value;
            characterEditorHandler.editingCharacter.emblem.layer2 = Layer2value;
            //update the emblem colors
            characterEditorHandler.editingCharacter.emblem.layer0Color = new CommonSaveDataStructure.SerializableColor(layer0color);
            characterEditorHandler.editingCharacter.emblem.layer1Color = new CommonSaveDataStructure.SerializableColor(layer1color);
            characterEditorHandler.editingCharacter.emblem.layer2Color = new CommonSaveDataStructure.SerializableColor(layer2color);
            //update the preview
            characterEditorHandler.UpdatePreview();

            CloseEditor();
        }

        public void RandomiseEmblem()
        {
            layer0field.Value = UnityEngine.Random.Range(0, layer0sprites.Length - 1);
            layer1field.Value = UnityEngine.Random.Range(0, layer1sprites.Length - 1);
            System.Random rand = new System.Random();
            layer2field.isOn = Convert.ToBoolean(rand.Next(0, 2));

            layer0color = UnityEngine.Random.ColorHSV();
            layer1color = UnityEngine.Random.ColorHSV();
            layer2color = UnityEngine.Random.ColorHSV();

            UpdatePreview();
        }

        #region ColorHandling

        public void EditColor(int index)
        {
            if (index == 1)
            {
                HSLColorPicker.instance.OpenColorPicker(SetColor1, primaryButton.color);
            }
            else if (index == 2)
            {
                HSLColorPicker.instance.OpenColorPicker(SetColor2, secondaryButton.color);
            }
            else if (index == 3)
            {
                HSLColorPicker.instance.OpenColorPicker(SetColor3, tertiaryButton.color);
            }
            else
            {
                Debug.Log("[GUI\\CharacterEditor] Invalid index provided for EditColor method.");
            }
        }

        public void UpdateColor(Color color, int index)
        {
            if (index == 1)
            {
                layer0color = color;
            }
            else if (index == 2)
            {
                layer1color = color;
            }
            else if (index == 3)
            {
                layer2color = color;
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

        #endregion
    }
}