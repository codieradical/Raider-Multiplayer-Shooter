using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

namespace Raider.Game.GUI.Components
{

    [RequireComponent(typeof(RectTransform))]
    public class GridSelectionSlider : MonoBehaviour
    {

        public GridLayoutGroup gridLayout; //The object with the GridLayoutGroup
        GameObject gridObject { get { return gridLayout.gameObject; } }
        List<GameObject> selectableObjects; //Objects within the GridLayoutGroup
        RectTransform rectTransform;

        public GameObject SelectedObject
        {
            get
            {
                foreach (GameObject selectableGameObject in selectableObjects)
                {
                    RectTransform _selectedObjectRectTransform = selectableGameObject.GetComponent<RectTransform>();
                    RectTransform _gridObjectRectTransform = gridObject.GetComponent<RectTransform>();

                    if (_gridObjectRectTransform.localPosition == -_selectedObjectRectTransform.localPosition)
                        return selectableGameObject;
                }
                Debug.LogError("[GUI/GridSelectionSlider] No Game Object Selected on " + name + ".");
                return null;
            }
            set
            {
                if (value.transform.parent == gridObject)
                    MoveSelection(value);
                else
                    Debug.LogWarning("[GUI/GridSelectionSlider] Something just tried to select an object off the grid!");
            }
        }

        void Start()
        {

            //If there's no gridObject, find one.
            if (gridLayout == null)
            {
                Debug.LogWarning("[GUI/GridSelectionSlider] No grid layout assigned on " + name + ", searching for a child with a GridLayoutGroup.");
                foreach (Transform child in transform)
                {
                    GridLayoutGroup childGridLayout = child.GetComponent<GridLayoutGroup>();
                    if (childGridLayout != null)
                        gridLayout = childGridLayout;
                }

                if (gridLayout == null)
                    Debug.LogError("[GUI/GridSelectionSlider] No grid layout found.");
                else
                    Debug.LogAssertion("[GUI/GridSelectionSlider] Found a suitable GridLayoutGroup on" + gridObject.name + ".");
            }

            rectTransform = GetComponent<RectTransform>();

            //Set the cell size.
            gridLayout.cellSize = rectTransform.rect.size;

            selectableObjects = new List<GameObject>();
            foreach (Transform childTransform in gridObject.transform)
            {
                //Get the LayoutElement component. 
                //Any child objects which ignore the layout group will be ignored.
                LayoutElement childLayoutElement = GetComponent<LayoutElement>();
                if (childLayoutElement != null)
                {
                    if (childLayoutElement.ignoreLayout == false)
                        selectableObjects.Add(childTransform.gameObject);
                }
                else
                    selectableObjects.Add(childTransform.gameObject);
            }

            foreach (GameObject selectableObject in selectableObjects)
            {
                selectableObject.AddComponent<Button>();
                Button _button = selectableObject.GetComponent<Button>();
                _button.onClick.AddListener(delegate { MoveSelection(_button); });
            }
        }

        public void MoveSelection(Button sender)
        {
            RectTransform _selectedObjectRectTransform = sender.gameObject.GetComponent<RectTransform>();
            RectTransform _gridObjectRectTransform = gridObject.GetComponent<RectTransform>();

            if (sender.gameObject.transform.parent.gameObject != gridObject)
            {
                Debug.LogWarning("[GUI/GridSelectionSlider] Something just tried to select an object based on a button off the grid!");
                return;
            }

            _gridObjectRectTransform.localPosition = -_selectedObjectRectTransform.localPosition;
        }

        void MoveSelection(GameObject selectedObject)
        {
            RectTransform _selectedObjectRectTransform = selectedObject.GetComponent<RectTransform>();
            RectTransform _gridObjectRectTransform = gridObject.GetComponent<RectTransform>();

            _gridObjectRectTransform.localPosition = -_selectedObjectRectTransform.localPosition;
        }
    }
}