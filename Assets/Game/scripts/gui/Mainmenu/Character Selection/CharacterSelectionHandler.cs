﻿using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.GUI.Components;
using Raider.Game.Saves.User;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
	public class CharacterSelectionHandler : MonoBehaviour
    {

        public Object characterPlatePrefab;
        public Object newPlatePrefab;

        public GameObject plateContainer;

        public const string PREVIEW_CHARACTER_NAME = "plate";
        const CharacterPreviewHandler.PreviewType PREVIEW_TYPE = CharacterPreviewHandler.PreviewType.Plate;

        private void Start()
        {
            Session.AddReloadHook(UserReloadedHook);
        }

        private void UserReloadedHook()
        {
            if (!Scene.Scenario.InLobby)
                return;

            if(MenuManager.instance.CurrentMenu == MainmenuController.instance.ChooseCharacterScreen.GetComponent<Menu>())
            {
                LoadCharacterPlates();
            }
        }


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
            if (Session.userSaveDataHandler.GetAllCharacters().Count < 1 && !platesPreviouslyExisted)
            {
                MainmenuController.instance.CreateCharacter();
                return;
            }

            //Create new plates
            int slot = 0;

            foreach (UserSaveDataStructure.Character character in Session.userSaveDataHandler.GetAllCharacters())
            {
                CreatePlate(character, slot);
                slot++;
            }

            InstanceNewCharacterPlate();
        }

        void CreatePlate(UserSaveDataStructure.Character character, int slot)
        {
            GameObject newPlate = Instantiate(characterPlatePrefab) as GameObject;

            newPlate.GetComponent<CharacterSelectionPlate>().selectionHandler = this;

            newPlate.transform.name = PREVIEW_CHARACTER_NAME + slot.ToString();
            newPlate.transform.SetParent(plateContainer.transform, false);
            newPlate.transform.Find("level").GetComponent<Text>().text = character.level.ToString();
            newPlate.transform.Find("Emblem").GetComponent<EmblemHandler>().UpdateEmblem(character);

            RawImage previewDisplay = newPlate.transform.Find("Image").GetComponent<RawImage>();
            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME + slot.ToString(), character, PREVIEW_TYPE, previewDisplay);

            Color plateColor = character.armourPrimaryColor.Color;
            plateColor.a = 0.5f;
            newPlate.GetComponent<Image>().color = plateColor;
        }

        void InstanceNewCharacterPlate()
        {
            GameObject newPlate = Instantiate(newPlatePrefab) as GameObject;
            newPlate.transform.SetParent(plateContainer.transform, false);

            newPlate.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(MainmenuController.instance.CreateCharacter));
        }

        public void ChooseCharacter(int slot)
        {
            if(Session.ActiveCharacter != null)
            {
                UserFeedback.LogError("Can't select a new character when you're already logged in!");
                Debug.LogError("Attempted to login to character when activeCharacter not null");
                return;
            }
            Session.SelectCharacter(slot);
            MainmenuController.instance.ChooseCharacter(slot);
        }

        public void DeleteCharacter(int slot)
        {
            Session.userSaveDataHandler.DeleteCharacter(slot, null, FailedToDeleteCharacterCallback);
            CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME + slot.ToString());
            LoadCharacterPlates();
        }

        public void FailedToDeleteCharacterCallback(string error)
        {
            UserFeedback.LogError(error);
            LoadCharacterPlates();
        }
    }
}