using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using Raider.Game.Saves;
using Raider.Game.GUI.Components;

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

        public bool layer2value { get { return layer2field.isOn; } }
        public int layer1value { get { return layer1field.value; } }
        public int layer0value { get { return layer0field.value; } }

        private Color layer0color;
        private Color layer1color;
        private Color layer2color;

        void Start()
        {
            gameObject.SetActive(false);

            layer0color = characterEditorHandler.editingCharacter.emblemLayer0Color.color;
            layer1color = characterEditorHandler.editingCharacter.emblemLayer1Color.color;
            layer2color = characterEditorHandler.editingCharacter.emblemLayer2Color.color;

            UpdatePreview();
        }

        public void OpenEditor()
        {
            //For some reason the object isn't activated until the second click, this is a temp fix.
            while (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void CloseEditor()
        {
            gameObject.SetActive(false);
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
            layer2image.gameObject.SetActive(layer2value);

            //Update layer images.
            layer0image.sprite = layer0sprites[layer0value];
            layer1image.sprite = layer1sprites[layer1value];
            layer2image.sprite = layer2sprites[layer1value];
        }

        public void Done()
        {
            //update the emblem images
            characterEditorHandler.editingCharacter.emblemLayer0 = layer0value;
            characterEditorHandler.editingCharacter.emblemLayer1 = layer1value;
            characterEditorHandler.editingCharacter.emblemLayer2 = layer2value;
            //update the emblem colors
            characterEditorHandler.editingCharacter.emblemLayer0Color = new SaveDataStructure.SerializableColor(layer0color);
            characterEditorHandler.editingCharacter.emblemLayer1Color = new SaveDataStructure.SerializableColor(layer1color);
            characterEditorHandler.editingCharacter.emblemLayer2Color = new SaveDataStructure.SerializableColor(layer2color);
            //update the preview
            characterEditorHandler.UpdatePreview();

            CloseEditor();
        }

        #region ColorHandling

        public void EditColor(int index)
        {
            if (index == 1)
            {
                HSLColorPicker.instance.OpenColorPicker(this, "SetColor1", primaryButton.color);
            }
            else if (index == 2)
            {
                HSLColorPicker.instance.OpenColorPicker(this, "SetColor2", secondaryButton.color);
            }
            else if (index == 3)
            {
                HSLColorPicker.instance.OpenColorPicker(this, "SetColor3", tertiaryButton.color);
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