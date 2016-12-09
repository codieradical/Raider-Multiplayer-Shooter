using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Raider.Game.GUI.Layout
{

    [AddComponentMenu("Layout/Layout Element Aspect Ratio Fitter", 142)]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(LayoutElement))]
    public class LayoutElementAspectFitter : AspectRatioFitter
    {
        [SerializeField]
        private AspectMode m_AspectMode = AspectMode.None;

        [SerializeField]
        private float m_AspectRatio = 1;

        [System.NonSerialized]
        private RectTransform m_Rect;

        private RectTransform AttachedRectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private LayoutElement AttachedLayoutElement
        {
            get
            {
                return GetComponent<LayoutElement>();
            }
        }

        private DrivenRectTransformTracker m_Tracker;

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(AttachedRectTransform);
            base.OnDisable();
        }

        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsActive())
                return;

            m_Tracker.Clear();

            switch (m_AspectMode)
            {
#if UNITY_EDITOR
                case AspectMode.None:
                    {
                        if (!Application.isPlaying)
                            m_AspectRatio = Mathf.Clamp(AttachedRectTransform.rect.width / AttachedRectTransform.rect.height, 0.001f, 1000f);

                        break;
                    }
#endif
                case AspectMode.HeightControlsWidth:
                    {
                        m_Tracker.Add(this, AttachedRectTransform, DrivenTransformProperties.SizeDeltaX);
                        AttachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, AttachedRectTransform.rect.height * m_AspectRatio);
                        break;
                    }
                case AspectMode.WidthControlsHeight:
                    {
                        m_Tracker.Add(this, AttachedRectTransform, DrivenTransformProperties.SizeDeltaY);
                        AttachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, AttachedRectTransform.rect.width / m_AspectRatio);
                        break;
                    }
                case AspectMode.FitInParent:
                case AspectMode.EnvelopeParent:
                    {
                        m_Tracker.Add(this, AttachedRectTransform,
                            DrivenTransformProperties.Anchors |
                            DrivenTransformProperties.AnchoredPosition |
                            DrivenTransformProperties.SizeDeltaX |
                            DrivenTransformProperties.SizeDeltaY);

                        AttachedRectTransform.anchorMin = Vector2.zero;
                        AttachedRectTransform.anchorMax = Vector2.one;
                        AttachedRectTransform.anchoredPosition = Vector2.zero;

                        Vector2 sizeDelta = Vector2.zero;
                        Vector2 parentSize = GetParentSize();
                        if ((parentSize.y * aspectRatio < parentSize.x) ^ (m_AspectMode == AspectMode.FitInParent))
                        {
                            sizeDelta.y = GetSizeDeltaToProduceSize(parentSize.x / aspectRatio, 1);
                        }
                        else
                        {
                            sizeDelta.x = GetSizeDeltaToProduceSize(parentSize.y * aspectRatio, 0);
                        }
                        AttachedRectTransform.sizeDelta = sizeDelta;

                        break;
                    }
            }
        }

        private float GetSizeDeltaToProduceSize(float size, int axis)
        {
            return size - GetParentSize()[axis] * (AttachedRectTransform.anchorMax[axis] - AttachedRectTransform.anchorMin[axis]);
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = AttachedRectTransform.parent as RectTransform;
            if (!parent)
                return Vector2.zero;
            return parent.rect.size;
        }

        public new virtual void SetLayoutHorizontal() { }

        public new virtual void SetLayoutVertical() { }

        protected new void SetDirty()
        {
            if (!IsActive())
                return;

            UpdateRect();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_AspectRatio = Mathf.Clamp(m_AspectRatio, 0.001f, 1000f);
            SetDirty();
        }

#endif
    }
}