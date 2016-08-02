using UnityEngine;
using System.Collections;

public class CharacterSelectionPlate : MonoBehaviour {

    public CharacterSelectionHandler selectionHandler;

    int slot
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
        selectionHandler.mainmenuHandler.EditCharacter(slot);
    }

    public void DeleteCharacter()
    {
        selectionHandler.DeleteCharacter(slot);
    }

    public void SelectCharacter()
    {
        selectionHandler.ChooseCharacter(slot);
    }
}
