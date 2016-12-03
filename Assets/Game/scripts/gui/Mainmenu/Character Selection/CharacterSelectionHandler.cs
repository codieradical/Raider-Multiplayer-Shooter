using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Raider.Game.Saves;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.CharacterPreviews;

namespace Raider.Game.GUI.Screens
{
    public class CharacterSelectionHandler : MonoBehaviour
    {

        public Object characterPlatePrefab;
        public Object newPlatePrefab;

        public GameObject plateContainer;

        public const string PREVIEW_CHARACTER_NAME = "plate";
        const CharacterPreviewHandler.PreviewType PREVIEW_TYPE = CharacterPreviewHandler.PreviewType.Plate;

        public void LoadCharacterPlates()
        {
            bool platesPreviouslyExisted = false;

            //Destroy all of the plates.
            foreach (Transform child in plateContainer.transform)
            {
                Destroy(child.gameObject);
                platesPreviouslyExisted = true;
            }

            CharacterPreviewHandler.instance.DestroyPreviews();

            //If no characters have been created go straight to the editor.
            if (Session.saveDataHandler.GetAllCharacters().Count < 1 && !platesPreviouslyExisted)
            {
                MainmenuHandler.instance.CreateCharacter();
                return;
            }

            //Create new plates
            int slot = 0;

            foreach (SaveDataStructure.Character character in Session.saveDataHandler.GetAllCharacters())
            {
                CreatePlate(character, slot);
                slot++;
            }

            InstanceNewCharacterPlate();
        }

        void CreatePlate(SaveDataStructure.Character character, int slot)
        {
            GameObject newPlate = Instantiate(characterPlatePrefab) as GameObject;

            newPlate.GetComponent<CharacterSelectionPlate>().selectionHandler = this;

            newPlate.transform.name = PREVIEW_CHARACTER_NAME + slot.ToString();
            newPlate.transform.SetParent(plateContainer.transform, false);
            newPlate.transform.FindChild("level").GetComponent<Text>().text = character.level.ToString();
            newPlate.transform.FindChild("Emblem").GetComponent<EmblemHandler>().UpdateEmblem(character);

            RawImage previewDisplay = newPlate.transform.FindChild("Image").GetComponent<RawImage>();
            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME + slot.ToString(), character, PREVIEW_TYPE, previewDisplay);

            Color plateColor = character.armourPrimaryColor.color;
            plateColor.a = 0.5f;
            newPlate.GetComponent<Image>().color = plateColor;
        }

        void InstanceNewCharacterPlate()
        {
            GameObject newPlate = Instantiate(newPlatePrefab) as GameObject;
            newPlate.transform.SetParent(plateContainer.transform, false);

            newPlate.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(MainmenuHandler.instance.CreateCharacter));
        }

        public void ChooseCharacter(int slot)
        {
            if(Session.activeCharacter != null)
            {
                UserFeedback.LogError("Can't select a new character when you're already logged in!");
                Debug.LogError("Attempted to login to character when activeCharacter not null");
                return;
            }
            Session.activeCharacter = Session.saveDataHandler.GetCharacter(slot);
            MainmenuHandler.instance.ChooseCharacter(slot);
        }

        public void DeleteCharacter(int slot)
        {
            Session.saveDataHandler.DeleteCharacter(slot);
            CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME + slot.ToString());
            LoadCharacterPlates();
        }
    }
}