using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPreviewHandler : MonoBehaviour {

    [Header("Plate Preview Prefabs")]
    public Object XPlatePreviewPrefab;
    public Object YPlatePreviewPrefab;


    [Header("Preview Prefabs")]
    public Object XPreviewPrefab;
    public Object YPreviewPrefab;

    int nextPreviewX = 0;

    public enum PreviewType
    {
        Full,
        Plate
    }

    //I needed a way to update preview appearence next frame, and I've been waiting for an excuse to use a Stack.
    private Stack<PreviewAppearenceUpdate> appearenceUpdates = new Stack<PreviewAppearenceUpdate>();

    private struct PreviewAppearenceUpdate
    {
        public PreviewAppearenceUpdate(string _previewName, SaveDataStructure.Character _character)
        {
            previewName = _previewName;
            character = _character;
        }

        public string previewName;
        public SaveDataStructure.Character character;
    }

    Object GetRacePreviewPrefab(SaveDataStructure.Character.Race _race, PreviewType _previewType)
    {
        switch (_race)
        {
            case SaveDataStructure.Character.Race.X:
                if (_previewType == PreviewType.Full)
                    return XPreviewPrefab;
                else
                    return XPlatePreviewPrefab;
            case SaveDataStructure.Character.Race.Y:
                if (_previewType == PreviewType.Full)
                    return YPreviewPrefab;
                else
                    return YPlatePreviewPrefab;
        }
        Debug.LogError("[GUI/CharacterPreviewHandler] Failed to get race prefab.");
        return XPreviewPrefab;
    }

    /// <summary>
    /// Creates a new preview character.
    /// </summary>
    /// <param name="_previewName">The name of the preview character.</param>
    /// <param name="_character">The character data associated with the preview.</param>
    /// <param name="_previewType">The type of preview to generate.</param>
    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType)
    {
        Object _prefab = GetRacePreviewPrefab(_character.race, _previewType);

        GameObject _newPreviewModel = Instantiate(_prefab) as GameObject;
        _newPreviewModel.transform.SetParent(transform, false);
        _newPreviewModel.transform.position += new Vector3(nextPreviewX, 0, 0);
        nextPreviewX += 250;
        _newPreviewModel.name = _previewName;

        PushPreviewUpdate(_previewName, _character);
    }

    #region NewPreview Overloads

    /// <summary>
    /// Creates a new preview character and returns it's camera.
    /// Useful for render texture setup.
    /// </summary>
    /// <param name="_previewName">The name of the preview character.</param>
    /// <param name="_character">The character data associated with the preview.</param>
    /// <param name="_previewType">The type of preview to generate.</param>
    /// <param name="previewCamera">The camera object attached to the preview prefab.</param>
    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, out Camera previewCamera)
    {
        Object _prefab = GetRacePreviewPrefab(_character.race, _previewType);

        GameObject _newPreviewModel = Instantiate(_prefab) as GameObject;
        _newPreviewModel.transform.SetParent(transform, false);
        _newPreviewModel.transform.position += new Vector3(nextPreviewX, 0, 0);
        nextPreviewX += 250;
        _newPreviewModel.name = _previewName;

        previewCamera = _newPreviewModel.transform.Find("cam").GetComponent<Camera>();

        PushPreviewUpdate(_previewName, _character);
    }

    /// <summary>
    /// Creates a new preview character, returns it's camera and assigns to the Display Handler provided.
    /// </summary>
    /// <param name="_previewName">The name of the preview character.</param>
    /// <param name="_character">The character data associated with the preview.</param>
    /// <param name="_previewType">The type of preview to generate.</param>
    /// <param name="previewCamera">The camera object attached to the preview prefab.</param>
    /// <param name="_displayHandler">The display handler which shows this character.</param>
    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, out Camera previewCamera, CharacterPreviewDisplayHandler _displayHandler)
    {
        Object _prefab = GetRacePreviewPrefab(_character.race, _previewType);

        GameObject _newPreviewModel = Instantiate(_prefab) as GameObject;
        _newPreviewModel.transform.SetParent(transform, false);
        _newPreviewModel.transform.position += new Vector3(nextPreviewX, 0, 0);
        nextPreviewX += 250;
        _newPreviewModel.name = _previewName;

        previewCamera = _newPreviewModel.transform.Find("cam").GetComponent<Camera>();

        PushPreviewUpdate(_previewName, _character);

        _displayHandler.previewCharacterGraphics = _newPreviewModel.transform.Find("Graphics").gameObject;
    }

    /// <summary>
    /// Creates a new preview character and assigns it to the DisplayHandler provided.
    /// </summary>
    /// <param name="_previewName">The name of the preview character.</param>
    /// <param name="_character">The character data associated with the preview.</param>
    /// <param name="_previewType">The type of preview to generate.</param>
    /// <param name="_displayHandler">The display handler which shows this character.</param>
    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, CharacterPreviewDisplayHandler _displayHandler)
    {
        Object _prefab = GetRacePreviewPrefab(_character.race, _previewType);

        GameObject _newPreviewModel = Instantiate(_prefab) as GameObject;
        _newPreviewModel.transform.SetParent(transform, false);
        _newPreviewModel.transform.position += new Vector3(nextPreviewX, 0, 0);
        nextPreviewX += 250;
        _newPreviewModel.name = _previewName;

        PushPreviewUpdate(_previewName, _character);

        _displayHandler.previewCharacterGraphics = _newPreviewModel.transform.Find("Graphics").gameObject;
    }

    #endregion

    public void PushPreviewUpdate(string _previewName, SaveDataStructure.Character _character)
    {
        appearenceUpdates.Push(new PreviewAppearenceUpdate(_previewName, _character));
    }

    void UpdatePreviewAppearence(PreviewAppearenceUpdate update)
    {
        //Find the graphics object, get the PlayerAppearenceController, call it's UpdatePlayerAppearence method.
        GameObject _previewGraphics = GetPreviewObject(update.previewName).transform.Find("Graphics").gameObject;
        PlayerAppearenceController _appearenceController = _previewGraphics.GetComponent<PlayerAppearenceController>();
        _appearenceController.UpdatePlayerAppearence(update.previewName, update.character);

        //Keep the PreviewID up to date.
        GetPreviewID(update.previewName).appearenceCharacter = update.character;
    }

    /// <summary>
    /// Searches for a Preview GameObject bu string.
    /// </summary>
    /// <param name="_previewName">The preview name to search for.</param>
    /// <returns></returns>
    public GameObject GetPreviewObject(string _previewName)
    {
        GameObject _previewObject = transform.Find(_previewName).gameObject;
        if (_previewObject == null)
            Debug.LogError("Could not find preview object " + _previewName);
        return _previewObject;
    }

    /// <summary>
    /// Searches for all Preview GameObjects with a name containing the provided string.
    /// </summary>
    /// <param name="_previewName">The string to search for.</param>
    /// <returns></returns>
    public List<GameObject> GetPreviewObjects(string _previewName)
    {
        List<GameObject> matchingPreviews = new List<GameObject>();
        foreach(Transform child in transform)
        {
            if (child.name.Contains(_previewName))
                matchingPreviews.Add(child.gameObject);
        }
        return matchingPreviews;
    }

    public void DestroyPreviewObject(string _previewName)
    {
        Destroy(GetPreviewObject(_previewName));
    }

    public Camera GetPreviewCamera(string _name)
    {
        return GetPreviewObject(_name).transform.Find("cam").GetComponent<Camera>();
    }

    public PreviewID GetPreviewID(string _previewName)
    {
        return GetPreviewObject(_previewName).GetComponent<PreviewID>();
    }

	// Use this for initialization
	void Start () {
	
        if(XPreviewPrefab == null || YPreviewPrefab == null)
        {
            Debug.LogError("[GUI/CharacterPreviewHandler] Missing a preview prefab");
        }
	}

    void LateUpdate()
    {
        while(appearenceUpdates.Count > 0)
        {
            UpdatePreviewAppearence(appearenceUpdates.Pop());
        }
    }
}
