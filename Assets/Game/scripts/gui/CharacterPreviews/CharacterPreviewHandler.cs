using Raider.Game.Saves.User;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.CharacterPreviews
{
    public class CharacterPreviewHandler : MonoBehaviour
    {

        #region Singleton Setup

        public static CharacterPreviewHandler instance;

        public void Awake()
        {
            if (instance != null)
                Debug.LogAssertion("It seems that multiple Character Preview Handlers are active, breaking the singleton instance.");
            instance = this;

            CheckAllPrefabsPresent();
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public void CheckAllPrefabsPresent()
        {
            foreach(UserSaveDataStructure.Character.Races race in Enum.GetValues(typeof(UserSaveDataStructure.Character.Races)))
            {
                List<PreviewType> foundPreviewTypes = new List<PreviewType>();
                foreach(PreviewRaceTypePrefab previewPrefab in previewPrefabs)
                {
                    if (previewPrefab.race == race)
                        foundPreviewTypes.Add(previewPrefab.type);
                }
                
                if(foundPreviewTypes.Count < 1)
                    Debug.LogWarning("Warning, no previews found for Race " + race.ToString());

                foreach (PreviewType previewType in Enum.GetValues(typeof(PreviewType)))
                {
                    bool foundPreviewType = false;
                    foreach(PreviewType foundType in foundPreviewTypes)
                    {
                        if (previewType == foundType)
                        {
                            if (foundPreviewType == true)
                                Debug.LogWarning("Warning, found more than one preview for Race " + race.ToString() + ", type " + previewType.ToString());
                            else
                                foundPreviewType = true;
                        }
                    }
                    if(!foundPreviewType)
                        Debug.LogWarning("Warning, no preview found for Race " + race.ToString() + ", type " + previewType.ToString());
                }
            }
        }

        [System.Serializable]
        private class PreviewRaceTypePrefab
        {
            [SerializeField] public UserSaveDataStructure.Character.Races race;
            [SerializeField] public PreviewType type;
            [SerializeField] public UnityEngine.Object prefab;
        }

        [SerializeField]
        private List<PreviewRaceTypePrefab> previewPrefabs = new List<PreviewRaceTypePrefab>();

        const string CAMERA_OBJECT_NAME = "cam";
        const string PREVIEW_MODEL_NAME = "Model";

        //Stores the position of the next preview. Increments as previews are created to keep them seperated.
        int nextPreviewX = 0;

        public enum PreviewType
        {
            Full,
            Plate
        }

        UnityEngine.Object GetRacePreviewPrefab(UserSaveDataStructure.Character.Races _race, PreviewType _previewType)
        {
            foreach(PreviewRaceTypePrefab previewPrefab in  previewPrefabs)
            {
                if (previewPrefab.race == _race && previewPrefab.type == _previewType)
                    return previewPrefab.prefab;
            }
            Debug.LogError("[GUI/CharacterPreviewHandler] Failed to get preview prefab for " + _race.ToString() + ", " + _previewType.ToString());
            return previewPrefabs[0].prefab;
        }

        #region create and setup previews

        public void NewPreview(string _previewName, UserSaveDataStructure.Character _previewCharacter, PreviewType _previewType, RawImage _rawImage)
        {
            Camera previewCamera;
            GameObject newPreviewModel; //This isn't actually used, I just don't want too many different overloads.
            InstanceNewPreviewObject(_previewName, _previewCharacter.Race, _previewType, out newPreviewModel, out previewCamera);
            SetupPreviewDisplay(previewCamera, _previewType, _rawImage);
            EnqueuePreviewUpdate(_previewName, _previewCharacter);
        }

        //Character Display Handler Overload.
        public void NewPreview(string _previewName, UserSaveDataStructure.Character _previewCharacter, PreviewType _previewType, RawImage _rawImage, CharacterPreviewDisplayHandler _displayHandler)
        {
            Camera newPreviewCamera;
            GameObject newPreviewModel;
            InstanceNewPreviewObject(_previewName, _previewCharacter.Race, _previewType, out newPreviewModel, out newPreviewCamera);
            SetupPreviewDisplay(newPreviewCamera, _previewType, _rawImage);
            SetupPreviewDisplayHandler(_displayHandler, newPreviewModel, newPreviewCamera);
            EnqueuePreviewUpdate(_previewName, _previewCharacter);
        }

        void InstanceNewPreviewObject(string _previewName, UserSaveDataStructure.Character.Races _race, PreviewType _previewType, out GameObject newPreviewModel, out Camera newPreviewCamera)
        {
            UnityEngine.Object prefab = GetRacePreviewPrefab(_race, _previewType);

            GameObject newPreviewObject = Instantiate(prefab) as GameObject;
            newPreviewObject.transform.SetParent(transform, false);
            newPreviewObject.transform.position += new Vector3(nextPreviewX, 0, 0);
            nextPreviewX += 100;

            newPreviewObject.name = _previewName;

            //It's important to pass these objects on now, as the usual method of finding these is unavailable just after instance.
            newPreviewModel = newPreviewObject.transform.Find(PREVIEW_MODEL_NAME).gameObject;
            newPreviewCamera = newPreviewObject.transform.Find(CAMERA_OBJECT_NAME).gameObject.GetComponent<Camera>();
        }

        void SetupPreviewDisplay(Camera _previewCamera, PreviewType _previewType, RawImage _rawImage)
        {
            RenderTexture newPreviewTexture;

            if (_previewType == PreviewType.Plate)
                newPreviewTexture = new RenderTexture((int)(Screen.height / 2.5), (int)(Screen.height / 2.5), 24, RenderTextureFormat.ARGB32);
            else if (_previewType == PreviewType.Full)
                newPreviewTexture = new RenderTexture(Screen.height, Screen.height, 24, RenderTextureFormat.ARGB32);
            else
            {
                Debug.LogError("[GUI/CharacterPreviewHandler] Unrecognized PreviewType, creating a very large Preview Texture.");
                newPreviewTexture = new RenderTexture(Screen.height, Screen.width, 24, RenderTextureFormat.ARGB32);
            }

            newPreviewTexture.Create();

            _previewCamera.targetTexture = newPreviewTexture;
            _rawImage.texture = newPreviewTexture;
        }

        void SetupPreviewDisplayHandler(CharacterPreviewDisplayHandler _displayHandler, GameObject newPreviewModel, Camera newPreviewCamera)
        {
            _displayHandler.previewCamera = newPreviewCamera;
            _displayHandler.previewCharacterModel = newPreviewModel;
        }

        #endregion

        #region update preview appearence.

        //A queue would be nice, but queues don't have Pop();
        private Queue<PreviewAppearenceUpdate> appearenceUpdates = new Queue<PreviewAppearenceUpdate>();

        private struct PreviewAppearenceUpdate
        {
            public PreviewAppearenceUpdate(string _previewName, UserSaveDataStructure.Character _previewCharacter)
            {
                previewName = _previewName;
                previewCharacter = _previewCharacter;
            }

            public string previewName;
            public UserSaveDataStructure.Character previewCharacter;
        }

        void LateUpdate()
        {
            while (appearenceUpdates.Count > 0)
            {
                UpdatePreviewAppearence(appearenceUpdates.Dequeue());
            }
        }

        public void EnqueuePreviewUpdate(string _previewName, UserSaveDataStructure.Character _character)
        {
            appearenceUpdates.Enqueue(new PreviewAppearenceUpdate(_previewName, _character));
        }

        void UpdatePreviewAppearence(PreviewAppearenceUpdate update)
        {
            //Find the object, get the PlayerAppearenceController, call it's UpdatePlayerAppearence method.
            GameObject previewModel = GetPreviewObject(update.previewName).transform.Find(PREVIEW_MODEL_NAME).gameObject;

            //If the preview was destored last frame, or earlier this frame.
            if (previewModel == null)
                return;

            CharacterPreviewAppearenceController _appearenceController = previewModel.GetComponent<CharacterPreviewAppearenceController>();
            _appearenceController.UpdatePlayerAppearence(update.previewCharacter);
            
        }

        #endregion

        #region interacting with existing previews

        /// <summary>
        /// Destroys all instanced previews and update data.
        /// </summary>
        public void DestroyPreviews()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            nextPreviewX = 0;
        }

        GameObject GetPreviewObject(string _previewName)
        {
            GameObject _previewObject = transform.Find(_previewName).gameObject;
            if (_previewObject == null)
                Debug.LogError("Could not find preview object " + _previewName);
            return _previewObject;
        }

        public GameObject GetPreviewModel(string _previewName)
        {
            GameObject model = GetPreviewObject(_previewName).transform.Find(PREVIEW_MODEL_NAME).gameObject;
            if (model == null)
                Debug.LogError("Could not find model object on " + _previewName);
            return model;
        }

        public void DestroyPreviewObject(string _previewName)
        {
            Destroy(GetPreviewObject(_previewName));
        }

        #endregion
    }
}