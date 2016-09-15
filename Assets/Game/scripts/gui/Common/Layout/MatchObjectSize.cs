using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Raider.Game.GUI.Layout
{

    [AddComponentMenu("Layout/Match Object Size")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class MatchObjectSize : MonoBehaviour
    {

        public GameObject matchGameObject;
        public bool matchWidth;
        public bool matchHeight;

        // Use this for initialization
        void Start()
        {
            if (matchGameObject == null)
            {
                Debug.Log(string.Format("[GUI] Match game object on {0} was provided no object to match", this.name));
            }
            else if (matchGameObject.GetComponent<RectTransform>() == null)
            {
                Debug.Log(string.Format("[GUI] {0} trying to match a game object with no rect transform ({1}).", this.name, matchGameObject.name));
            }
        }

        // Update is called once per frame

        void ResizeUI()
        {
            RectTransform matchRect = matchGameObject.GetComponent<RectTransform>();

            if (matchRect != null)
            {
                RectTransform currentRect = GetComponent<RectTransform>();
                Vector2 updatedSize = currentRect.rect.size;
                if (matchWidth)
                {
                    updatedSize.x = matchRect.rect.size.x;
                }
                if (matchHeight)
                {
                    updatedSize.y = matchRect.rect.size.y;
                }

                currentRect.sizeDelta = updatedSize;
            }
        }

        void Update()
        {
            ResizeUI();
        }

#if UNITY_EDITOR
        void OnRenderObject()
        {
            ResizeUI();
            //LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        }
#endif
    }
}