using Raider.Game.GUI.Screens;
using UnityEngine;

namespace Raider.Game.GUI.Components
{
	public class CharacterSelectionPlate : MonoBehaviour
    {
        public CharacterSelectionHandler selectionHandler;
        int Slot
        {
            get
            {
                string unparsed = name.Replace(CharacterSelectionHandler.PREVIEW_CHARACTER_NAME, "");
                try
                {
                    return int.Parse(unparsed);
                }
                catch
                {
                    Debug.LogError("[GUI/CharacterSelectionPlate] Failed to parse plate slot.");
                    return 0;
                }
            }
        }

        public void EditCharacter()
        {
            MainmenuController.instance.EditCharacter(Slot);
        }

        public void DeleteCharacter()
        {
            selectionHandler.DeleteCharacter(Slot);
        }

        public void SelectCharacter()
        {
            selectionHandler.ChooseCharacter(Slot);
        }
    }
}