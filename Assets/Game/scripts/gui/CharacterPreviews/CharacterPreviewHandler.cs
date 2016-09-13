using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterPreviewHandler : MonoBehaviour {

    #region Singleton Setup

    public static CharacterPreviewHandler instance;

    public void Awake()
    {
        if (instance != null)
            Debug.LogAssertion("It seems that multiple Character Preview Handlers are active, breaking the singleton instance.");
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    #endregion

    [Header("Plate Preview Prefabs")]
    public Object XPlatePreviewPrefab;
    public Object YPlatePreviewPrefab;


    [Header("Preview Prefabs")]
    public Object XPreviewPrefab;
    public Object YPreviewPrefab;

    const string CAMERA_OBJECT_NAME = "cam";
    const string GRAPHICS_OBJECT_NAME = "Graphics";

    //Stores the position of the next preview. Increments as previews are created to keep them seperated.
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

    #region NewPreview Overloads

    void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, out GameObject _newPreviewModel, out GameObject _graphicsObject, out Camera _camera, out PreviewID _previewID)
    {
        Object prefab = GetRacePreviewPrefab(_character.race, _previewType);

        GameObject newPreviewModel = Instantiate(prefab) as GameObject;
        newPreviewModel.transform.SetParent(transform, false);
        newPreviewModel.transform.position += new Vector3(nextPreviewX, 0, 0);
        nextPreviewX += 250;
        newPreviewModel.name = _previewName;

        _graphicsObject = newPreviewModel.transform.Find(GRAPHICS_OBJECT_NAME).gameObject;
        _camera = newPreviewModel.transform.Find(CAMERA_OBJECT_NAME).gameObject.GetComponent<Camera>();
        _previewID = newPreviewModel.GetComponent<PreviewID>();

        PushPreviewUpdate(_previewName, _character);

        _newPreviewModel = newPreviewModel;
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType)
    {
        GameObject _newPreviewModel, _graphicsObject;
        Camera _camera;
        PreviewID _previewID;

        NewPreview(_previewName, _character, _previewType, out _newPreviewModel, out _graphicsObject, out _camera, out _previewID);

        UpdatePreviewID(_previewID, _previewType, _character, _graphicsObject, _camera);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, int _slot)
    {
        GameObject _newPreviewModel, _graphicsObject;
        Camera _camera;
        PreviewID _previewID;

        NewPreview(_previewName, _character, _previewType, out _newPreviewModel, out _graphicsObject, out _camera, out _previewID);

        UpdatePreviewID(_previewID, _previewType, _character, _graphicsObject, _camera, _slot);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, string _player)
    {
        GameObject _newPreviewModel, _graphicsObject;
        Camera _camera;
        PreviewID _previewID;

        NewPreview(_previewName, _character, _previewType, out _newPreviewModel, out _graphicsObject, out _camera, out _previewID);

        UpdatePreviewID(_previewID, _previewType, _character, _graphicsObject, _camera, _player);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, int _slot, string _player)
    {
        GameObject _newPreviewModel, _graphicsObject;
        Camera _camera;
        PreviewID _previewID;

        NewPreview(_previewName, _character, _previewType, out _newPreviewModel, out _graphicsObject, out _camera, out _previewID);

        UpdatePreviewID(_previewID, _previewType, _character, _graphicsObject, _camera, _player, _slot);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, CharacterPreviewDisplayHandler _displayHandler)
    {
        NewPreview(_previewName, _character, _previewType, _displayHandler.gameObject.GetComponent<RawImage>());

        _displayHandler.previewCharacterGraphics = GetPreviewGraphics(_previewName);
        _displayHandler.previewCamera = GetPreviewCamera(_previewName);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, CharacterPreviewDisplayHandler _displayHandler, int _slot)
    {
        NewPreview(_previewName, _character, _previewType, _displayHandler.gameObject.GetComponent<RawImage>(), _slot);

        _displayHandler.previewCharacterGraphics = GetPreviewGraphics(_previewName);
        _displayHandler.previewCamera = GetPreviewCamera(_previewName);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, CharacterPreviewDisplayHandler _displayHandler, int _slot, string _player)
    {
        NewPreview(_previewName, _character, _previewType, _displayHandler.gameObject.GetComponent<RawImage>(), _slot, _player);

        _displayHandler.previewCharacterGraphics = GetPreviewGraphics(_previewName);
        _displayHandler.previewCamera = GetPreviewCamera(_previewName);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, CharacterPreviewDisplayHandler _displayHandler, string _player)
    {
        NewPreview(_previewName, _character, _previewType, _displayHandler.gameObject.GetComponent<RawImage>(), _player);

        _displayHandler.previewCharacterGraphics = GetPreviewGraphics(_previewName);
        _displayHandler.previewCamera = GetPreviewCamera(_previewName);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, RawImage _rawImage)
    {
        NewPreview(_previewName, _character, _previewType);
        SetupPreviewTexture(GetPreviewCamera(_previewName), _previewType, _rawImage);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, RawImage _rawImage, int _slot)
    {
        NewPreview(_previewName, _character, _previewType, _slot);
        SetupPreviewTexture(GetPreviewCamera(_previewName), _previewType, _rawImage);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, RawImage _rawImage, int _slot, string _player)
    {
        NewPreview(_previewName, _character, _previewType, _slot, _player);
        SetupPreviewTexture(GetPreviewCamera(_previewName), _previewType, _rawImage);
    }

    public void NewPreview(string _previewName, SaveDataStructure.Character _character, PreviewType _previewType, RawImage _rawImage, string _player)
    {
        NewPreview(_previewName, _character, _previewType, _player);
        SetupPreviewTexture(GetPreviewCamera(_previewName), _previewType, _rawImage);
    }

    /// <summary>
    /// Renders a preview camera to a new texture and displays that texture on the input _rawImage.
    /// </summary>
    /// <param name="_previewCamera">The preview camera to render on the texture.</param>
    /// <param name="_previewType">The type of preview to render. This is important for creating an appropriately sized texture.</param>
    /// <param name="_rawImage">The raw image to display the texture on.</param>
    void SetupPreviewTexture(Camera _previewCamera, PreviewType _previewType, RawImage _rawImage)
    {
        RenderTexture previewCharacterTexture;

        if (_previewType == PreviewType.Plate)
            previewCharacterTexture = new RenderTexture(Screen.height / 3, Screen.height / 3, 24, RenderTextureFormat.ARGB32);
        else if (_previewType == PreviewType.Full)
            previewCharacterTexture = new RenderTexture(Screen.height / 3, Screen.height / 3, 24, RenderTextureFormat.ARGB32);
        else
        {
            Debug.LogError("[GUI/CharacterPreviewHandler] Unrecognized PreviewType, creating a very large Preview Texture.");
            previewCharacterTexture = new RenderTexture(Screen.height, Screen.width, 24, RenderTextureFormat.ARGB32);
        }

        previewCharacterTexture.Create();

        _previewCamera.targetTexture = previewCharacterTexture;
        _rawImage.texture = previewCharacterTexture;
    }

    void UpdatePreviewID(PreviewID _previewID, PreviewType _previewType, SaveDataStructure.Character _character, GameObject _graphicsObject, Camera _cameraObject)
    {
        _previewID.previewType = _previewType;
        _previewID.appearenceCharacter = _character;
        _previewID.graphicsObject = _graphicsObject;
        _previewID.camera = _cameraObject;
    }

    void UpdatePreviewID(PreviewID _previewID, PreviewType _previewType, SaveDataStructure.Character _character, GameObject _graphicsObject, Camera _cameraObject, string _player)
    {
        UpdatePreviewID(_previewID, _previewType, _character, _graphicsObject, _cameraObject);
        _previewID.player = _player;
    }

    void UpdatePreviewID(PreviewID _previewID, PreviewType _previewType, SaveDataStructure.Character _character, GameObject _graphicsObject, Camera _cameraObject, int _slot)
    {
        _previewID.slot = _slot;
    }

    void UpdatePreviewID(PreviewID _previewID, PreviewType _previewType, SaveDataStructure.Character _character, GameObject _graphicsObject, Camera _cameraObject, string _player, int _slot)
    {
        UpdatePreviewID(_previewID, _previewType, _character, _graphicsObject, _cameraObject, _player);
        _previewID.slot = _slot;
    }

    #endregion

    public void PushPreviewUpdate(string _previewName, SaveDataStructure.Character _character)
    {
        appearenceUpdates.Push(new PreviewAppearenceUpdate(_previewName, _character));
    }

    void UpdatePreviewAppearence(PreviewAppearenceUpdate update)
    {
        //Find the graphics object, get the PlayerAppearenceController, call it's UpdatePlayerAppearence method.
        GameObject _previewGraphics = GetPreviewObject(update.previewName).transform.Find(GRAPHICS_OBJECT_NAME).gameObject;
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
    GameObject GetPreviewObject(string _previewName)
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
    List<GameObject> GetPreviewObjects(string _previewName)
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

    public void DestroyPreviewObjects(string _previewName)
    {
        foreach(GameObject preview in GetPreviewObjects(_previewName))
        {
            Destroy(preview);
        }
    }

    public Camera GetPreviewCamera(string _name)
    {
        return GetPreviewID(_name).camera;
    }

    public GameObject GetPreviewGraphics(string _name)
    {
        return GetPreviewID(_name).graphicsObject;
    }

    PreviewID GetPreviewID(string _previewName)
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
