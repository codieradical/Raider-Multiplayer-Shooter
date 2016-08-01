using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPreviewHandler : MonoBehaviour {

    public Object XPreviewPrefab;
    public Object YPreviewPrefab;

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

    Object GetRacePreviewPrefab(SaveDataStructure.Character.Race _race)
    {
        switch (_race)
        {
            case SaveDataStructure.Character.Race.X:
                return XPreviewPrefab;
            case SaveDataStructure.Character.Race.Y:
                return YPreviewPrefab;
        }
        Debug.LogError("[GUI/CharacterPreviewHandler] Failed to get race prefab.");
        return XPreviewPrefab;
    }

    /// <summary>
    /// Creates a new preview character.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_character"></param>
    public void NewPreview(string _name, SaveDataStructure.Character _character)
    {
        Object _prefab = GetRacePreviewPrefab(_character.race);

        GameObject _newPreviewModel = Instantiate(_prefab) as GameObject;
        _newPreviewModel.transform.SetParent(transform, false);
        _newPreviewModel.name = _name;

        PushPreviewUpdate(_name, _character);
    }

    /// <summary>
    /// Creates a new preview character and returns it's camera.
    /// Useful for render texture setup.
    /// </summary>
    /// <param name="_previewName"></param>
    /// <param name="_character"></param>
    /// <param name="previewCamera"></param>
    public void NewPreview(string _previewName, SaveDataStructure.Character _character, out Camera previewCamera)
    {
        Object _prefab = GetRacePreviewPrefab(_character.race);

        GameObject _newPreviewModel = Instantiate(_prefab) as GameObject;
        _newPreviewModel.transform.SetParent(transform, false);
        _newPreviewModel.name = _previewName;

        previewCamera = _newPreviewModel.transform.Find("cam").GetComponent<Camera>();

        PushPreviewUpdate(_previewName, _character);
    }

    public void PushPreviewUpdate(string _previewName, SaveDataStructure.Character _character)
    {
        appearenceUpdates.Push(new PreviewAppearenceUpdate(_previewName, _character));
    }

    public void PushPreviewUpdate(string _previewName, SaveDataStructure.Character _character, bool changeRace)
    {
        if (changeRace)
        {
            DestroyPreviewObject(_previewName);
            NewPreview(_previewName, _character);
        }
        else
            PushPreviewUpdate(_previewName, _character);
    }

    void UpdatePreviewAppearence(PreviewAppearenceUpdate update)
    {
        GameObject _previewGraphics = GetPreviewObject(update.previewName).transform.Find("Graphics").gameObject;
        PlayerAppearenceController _appearenceController = _previewGraphics.GetComponent<PlayerAppearenceController>();
        _appearenceController.UpdatePlayerAppearence(update.previewName, update.character);
    }

    public GameObject GetPreviewObject(string _previewName)
    {
        GameObject _previewObject = transform.Find(_previewName).gameObject;
        if (_previewObject == null)
            Debug.LogError("Could not find preview object " + _previewName);
        return _previewObject;
    }

    public void DestroyPreviewObject(string _previewName)
    {
        Destroy(GetPreviewObject(_previewName));
    }

    public Camera GetPreviewCamera(string _name)
    {
        return GetPreviewObject(_name).transform.Find("cam").GetComponent<Camera>();
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
