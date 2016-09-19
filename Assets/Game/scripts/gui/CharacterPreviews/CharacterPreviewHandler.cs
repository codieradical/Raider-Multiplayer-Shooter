using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Raider.Game.Saves;
using Raider.Game.Player;

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

            if (XPlatePreviewPrefab == null || YPlatePreviewPrefab == null || XPreviewPrefab == null || YPreviewPrefab == null)
                Debug.LogError("Character Preview Handler is missing a prefab!");
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

        #region create and setup previews

        public void NewPreview(string _previewName, SaveDataStructure.Character _previewCharacter, PreviewType _previewType, RawImage _rawImage)
        {
            Camera previewCamera;
            GameObject newPreviewGraphics; //This isn't actually used, I just don't want too many different overloads.
            InstanceNewPreviewObject(_previewName, _previewCharacter.race, _previewType, out newPreviewGraphics, out previewCamera);
            SetupPreviewDisplay(previewCamera, _previewType, _rawImage);
            PushPreviewUpdate(_previewName, _previewCharacter);
        }

        //Character Display Handler Overload.
        public void NewPreview(string _previewName, SaveDataStructure.Character _previewCharacter, PreviewType _previewType, RawImage _rawImage, CharacterPreviewDisplayHandler _displayHandler)
        {
            Camera newPreviewCamera;
            GameObject newPreviewGraphics;
            InstanceNewPreviewObject(_previewName, _previewCharacter.race, _previewType, out newPreviewGraphics, out newPreviewCamera);
            SetupPreviewDisplay(newPreviewCamera, _previewType, _rawImage);
            SetupPreviewDisplayHandler(_displayHandler, newPreviewGraphics, newPreviewCamera);
            PushPreviewUpdate(_previewName, _previewCharacter);
        }

        void InstanceNewPreviewObject(string _previewName, SaveDataStructure.Character.Race _race, PreviewType _previewType, out GameObject newPreviewGraphics, out Camera newPreviewCamera)
        {
            Object prefab = GetRacePreviewPrefab(_race, _previewType);

            GameObject newPreviewObject = Instantiate(prefab) as GameObject;
            newPreviewObject.transform.SetParent(transform, false);
            newPreviewObject.transform.position += new Vector3(nextPreviewX, 0, 0);
            nextPreviewX += 100;

            newPreviewObject.name = _previewName;

            //It's important to pass these objects on now, as the usual method of finding these is unavailable just after instance.
            newPreviewGraphics = newPreviewObject.transform.Find(GRAPHICS_OBJECT_NAME).gameObject;
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

        void SetupPreviewDisplayHandler(CharacterPreviewDisplayHandler _displayHandler, GameObject newPreviewGraphics, Camera newPreviewCamera)
        {
            _displayHandler.previewCamera = newPreviewCamera;
            _displayHandler.previewCharacterGraphics = newPreviewGraphics;
        }

        #endregion

        #region update preview appearence.

        //A queue would be nice, but queues don't have Pop();
        private Stack<PreviewAppearenceUpdate> appearenceUpdates = new Stack<PreviewAppearenceUpdate>();

        private struct PreviewAppearenceUpdate
        {
            public PreviewAppearenceUpdate(string _previewName, SaveDataStructure.Character _previewCharacter)
            {
                previewName = _previewName;
                previewCharacter = _previewCharacter;
            }

            public string previewName;
            public SaveDataStructure.Character previewCharacter;
        }

        void LateUpdate()
        {
            while (appearenceUpdates.Count > 0)
            {
                UpdatePreviewAppearence(appearenceUpdates.Pop());
            }
        }

        public void PushPreviewUpdate(string _previewName, SaveDataStructure.Character _character)
        {
            appearenceUpdates.Push(new PreviewAppearenceUpdate(_previewName, _character));
        }

        void UpdatePreviewAppearence(PreviewAppearenceUpdate update)
        {
            //Find the graphics object, get the PlayerAppearenceController, call it's UpdatePlayerAppearence method.
            GameObject _previewGraphics = GetPreviewObject(update.previewName).transform.Find(GRAPHICS_OBJECT_NAME).gameObject;

            //If the preview was destored last frame, or earlier this frame.
            if (_previewGraphics == null)
                return;

            PlayerAppearenceController _appearenceController = _previewGraphics.GetComponent<PlayerAppearenceController>();
            _appearenceController.UpdatePlayerAppearence(update.previewName, update.previewCharacter);
            
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

        public GameObject GetPreviewGraphics(string _previewName)
        {
            GameObject graphics = GetPreviewObject(_previewName).transform.Find(CAMERA_OBJECT_NAME).gameObject;
            if (graphics == null)
                Debug.LogError("Could not find graphics object on " + _previewName);
            return graphics;
        }

        public void DestroyPreviewObject(string _previewName)
        {
            Destroy(GetPreviewObject(_previewName));
        }

        #endregion
    }
}