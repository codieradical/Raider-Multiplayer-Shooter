using Raider.Game.GUI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Raider.Game.Scene;
using Raider.Game.Networking;
using Raider.Game.GUI.CharacterPreviews;
using UnityEngine.UI;
using Raider.Game.Saves;
using Raider.Game.Player;

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
            perspectiveSelection.title.text = "Perspective: " + Session.saveDataHandler.GetSettings().perspective.ToString();

            switch (Session.saveDataHandler.GetSettings().perspective)
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
            CharacterPreviewHandler.instance.NewPreview(PREVIEW_CHARACTER_NAME, Session.activeCharacter, PREVIEW_TYPE, characterPreviewRawImage, characterPreviewImage.GetComponent<CharacterPreviewDisplayHandler>());
        }

        void UpdatePerspectiveSelection(GameObject newObject)
        {
            SaveDataStructure.Settings settings = Session.saveDataHandler.GetSettings();
            switch(newObject.name)
            {
                case "FirstPerson":
                    settings.perspective = Cameras.CameraModeController.CameraModes.FirstPerson;
                    break;
                case "ThirdPerson":
                    settings.perspective = Cameras.CameraModeController.CameraModes.ThirdPerson;
                    break;
                case "Shoulder":
                    settings.perspective = Cameras.CameraModeController.CameraModes.Shoulder;
                    break;
            }
            if (!Scenario.InLobby)
                PlayerData.localPlayerData.playerManager.UpdatePerspective(settings.perspective);

            Session.saveDataHandler.SaveSettings(settings);
            perspectiveSelection.title.text = "Perspective: " + Session.saveDataHandler.GetSettings().perspective.ToString();
        }

        public override void ClosePane()
        {
            //Destroy the player preview...
            CharacterPreviewHandler.instance.DestroyPreviewObject(PREVIEW_CHARACTER_NAME);
            base.ClosePane();
        }

    }
}