using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Layout
{

    [AddComponentMenu("Layout/Preferred Size Override")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(LayoutElement))]
    public class PreferredSizeOverride : MonoBehaviour
    {

        public GameObject providedGameObject;
        public float Percentage;

        public enum OverrideTypes
        {
            None,
            PercentageOfParentWidth,
            PercentageOfParentHeight,
            PercentageOfObjectWidth,
            PercentageOfObjectHeight,
            Width,
            Height
        }

        public OverrideTypes widthOverride;
        public OverrideTypes heightOverride;

        void ApplyOverride()
        {
            RectTransform rt = GetComponent<RectTransform>();
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            RectTransform parentRT = transform.parent.GetComponent<RectTransform>();
            RectTransform objRT = null;
            if (providedGameObject != null)
                objRT = providedGameObject.GetComponent<RectTransform>();
            Vector2 newSize = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);

            switch (widthOverride)
            {
                case OverrideTypes.None:
                    break;
                case OverrideTypes.PercentageOfParentHeight:
                    newSize.x = parentRT.rect.size.y * Percentage;
                    break;
                case OverrideTypes.PercentageOfParentWidth:
                    newSize.x = parentRT.rect.size.x * Percentage;
                    break;
                case OverrideTypes.PercentageOfObjectHeight:
                    newSize.x = objRT.rect.size.y * Percentage;
                    break;
                case OverrideTypes.PercentageOfObjectWidth:
                    newSize.x = objRT.rect.size.x * Percentage;
                    break;
                case OverrideTypes.Height:
                    newSize.x = rt.rect.size.y;
                    break;
            }
            switch (heightOverride)
            {
                case OverrideTypes.None:
                    break;
                case OverrideTypes.PercentageOfParentHeight:
                    newSize.y = parentRT.rect.size.y * Percentage;
                    break;
                case OverrideTypes.PercentageOfParentWidth:
                    newSize.y = parentRT.rect.size.x * Percentage;
                    break;
                case OverrideTypes.PercentageOfObjectHeight:
                    newSize.y = objRT.rect.size.y * Percentage;
                    break;
                case OverrideTypes.PercentageOfObjectWidth:
                    newSize.y = objRT.rect.size.x * Percentage;
                    break;
                case OverrideTypes.Width:
                    newSize.y = rt.rect.size.x;
                    break;
            }

            layoutElement.preferredWidth = newSize.x;
            layoutElement.preferredHeight = newSize.y;
        }

        void Update()
        {
            ApplyOverride();
        }

#if UNITY_EDITOR
        void OnRenderObject()
        {
            ApplyOverride();
        }
#endif
    }
}