using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.GUI.Components;
using Raider.Game.Player;
using Raider.Game.Saves.User;
using Raider.Game.Scene;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.StartMenu
{
	public class StartMenuPlayer : StartMenuPane
    {
        public GridSelectionSlider perspectiveSelection;

        public GameObject characterPreviewImage;
        RawImage characterPreviewRawImage;

        const string PREVIEW_CHARACTER_NAME = "StartMenu";
        const CharacterPreviewHandler.PreviewType PREVIEW_TYPE = CharacterPreviewHandler.PreviewType.Full;

        void Start()
        {
            characterPreviewRawImage = characterPreviewImage.GetComponent<RawImage>();
        }

        protected override void SetupPaneData()
        {
            perspectiveSelection.onSelectionChanged = UpdatePerspectiveSelection;
            perspectiveSelection.title.text = "Perspective: " + Session.userSaveDataHandler.GetSettings().Perspective.ToString();

            switch (Session.userSaveDataHandler.GetSettings().Perspective)
            {
                case Cameras.CameraModeController.CameraModes.FirstPerson:
                    perspectiveSelection.SelectedObject = perspectiveSelection.gridLayout.transform.Find("FirstPerson").gameObject;
                    break;
                case Cameras.CameraModeController.CameraModes.ThirdPerson:
                    perspectiveSelection.SelectedObject = perspectiveSelection.gridLayout.transform.Find("ThirdPerson").gameObject;
                    break;
                case Cameras.CameraModeController.CameraModes.Shoulder:
                    perspectiveSelection.SelectedObject = perspectiveSelection.gridLayout.transform.Find("Shoulder").gameObject;
                    break;
            }

            StartCoroutine(SetupPlayerPreviewAfterAFrame());
        }

        IEnumerator SetupPlayerPreviewAfterAFrame()
        {
            yield return 0;
            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, Session.ActiveCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());
        }

        void UpdatePerspectiveSelection(GameObject newObject)
        {
            UserSaveDataStructure.UserSettings settings = Session.userSaveDataHandler.GetSettings();
            switch(newObject.name)
            {
                case "FirstPerson":
                    settings.Perspective = Cameras.CameraModeController.CameraModes.FirstPerson;
                    break;
                case "ThirdPerson":
                    settings.Perspective = Cameras.CameraModeController.CameraModes.ThirdPerson;
                    break;
                case "Shoulder":
                    settings.Perspective = Cameras.CameraModeController.CameraModes.Shoulder;
                    break;
            }
            if (!Scenario.InLobby)
                PlayerData.localPlayerData.localPlayerController.UpdatePerspective(settings.Perspective);

            Session.userSaveDataHandler.SaveSettings(settings, null, FailedToSaveSettingsCallback);
            perspectiveSelection.title.text = "Perspective: " + Session.userSaveDataHandler.GetSettings().Perspective.ToString();
        }

        public void FailedToSaveSettingsCallback(string error)
        {
            Debug.Log("Failed to save user perspective settings. \n" + error);
        }

        public override void ClosePane()
        {
            //Destroy the player preview...
            CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);
            base.ClosePane();
        }

    }
}