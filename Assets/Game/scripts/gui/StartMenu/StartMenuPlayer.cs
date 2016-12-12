using Raider.Game.GUI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Raider.Game.Scene;
using Raider.Game.Networking;

namespace Raider.Game.GUI.StartMenu
{
    public class StartMenuPlayer : StartMenuPane
    {
        public GridSelectionSlider perspectiveSelection;

        protected override void SetupPaneData()
        {
            perspectiveSelection.onSelectionChanged = UpdatePerspectiveSelection;
            perspectiveSelection.title.text = "Perspective: " + Session.activeCharacter.chosenPlayerPerspective.ToString();

            switch(Session.activeCharacter.chosenPlayerPerspective)
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
        }

        void UpdatePerspectiveSelection(GameObject newObject)
        {
            switch(newObject.name)
            {
                case "FirstPerson":
                    Session.activeCharacter.chosenPlayerPerspective = Cameras.CameraModeController.CameraModes.FirstPerson;
                    break;
                case "ThirdPerson":
                    Session.activeCharacter.chosenPlayerPerspective = Cameras.CameraModeController.CameraModes.ThirdPerson;
                    break;
                case "Shoulder":
                    Session.activeCharacter.chosenPlayerPerspective = Cameras.CameraModeController.CameraModes.Shoulder;
                    break;
            }
            if (!Scenario.InLobby)
                Player.Player.localPlayer.UpdatePerspective(Session.activeCharacter.chosenPlayerPerspective);
            else if (LobbyPlayerData.localPlayer != null)
                LobbyPlayerData.localPlayer.character.chosenPlayerPerspective = Session.activeCharacter.chosenPlayerPerspective;

            Session.SaveActiveCharacter();
            perspectiveSelection.title.text = "Perspective: " + Session.activeCharacter.chosenPlayerPerspective.ToString();
        }

    }
}