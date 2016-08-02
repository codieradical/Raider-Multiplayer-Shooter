using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectionHandler : MonoBehaviour {

    //Assigned by MainMenu Handler.
    [HideInInspector]
    public CharacterPreviewHandler characterPreviewHandler;
    [HideInInspector]
    public MainmenuHandler mainmenuHandler;

    public Object characterPlatePrefab;
    public Object newPlatePrefab;

    public GameObject plateContainer;

    public const string PREVIEW_CHARACTER_NAME = "plate";

    public void LoadCharacterPlates()
    {
        //Destroy all of the plates.
        foreach(Transform child in plateContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(GameObject preview in characterPreviewHandler.GetPreviewObjects(PREVIEW_CHARACTER_NAME))
        {
            Destroy(preview);
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
        SetupPreviewImage(character, previewDisplay, slot);

        Color plateColor = character.armourPrimaryColor.color;
        plateColor.a = 0.5f;
        newPlate.GetComponent<Image>().color = plateColor;
    }

    void InstanceNewCharacterPlate()
    {
        GameObject newPlate = Instantiate(newPlatePrefab) as GameObject;
        newPlate.transform.SetParent(plateContainer.transform, false);

        newPlate.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(mainmenuHandler.CreateCharacter));
    }

    void SetupPreviewImage(SaveDataStructure.Character _character, RawImage plateImage, int characterSlot)
    {
        Camera previewCamera;

        characterPreviewHandler.NewPreview(PREVIEW_CHARACTER_NAME + characterSlot.ToString(), _character, CharacterPreviewHandler.PreviewType.Plate, out previewCamera);

        RenderTexture previewCharacterImage = new RenderTexture(Screen.height / 3, Screen.height / 3, 24, RenderTextureFormat.ARGB32);
        previewCharacterImage.Create();

        previewCamera.targetTexture = previewCharacterImage;
        plateImage.GetComponent<RawImage>().texture = previewCharacterImage;
    }

    public void ChooseCharacter(int slot)
    {
        Session.character = Session.saveDataHandler.GetCharacter(slot);
        mainmenuHandler.ChooseCharacter(slot);
    }

    public void DeleteCharacter(int slot)
    {
        Session.saveDataHandler.DeleteCharacter(slot);
        Destroy(characterPreviewHandler.GetPreviewObject(PREVIEW_CHARACTER_NAME + slot.ToString()));
        LoadCharacterPlates();
    }
}
