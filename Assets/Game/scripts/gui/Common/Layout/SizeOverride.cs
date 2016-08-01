using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Layout/Size Override")]
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class SizeOverride : MonoBehaviour {

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
        RectTransform parentRT = transform.parent.GetComponent<RectTransform>();
        RectTransform objRT = null;
        if(providedGameObject != null)
            objRT = providedGameObject.GetComponent<RectTransform>();
        Vector2 newSize = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);

        switch(widthOverride)
        {
            case OverrideTypes.None:
                break;
            case OverrideTypes.PercentageOfParentHeight:
                newSize.x = parentRT.sizeDelta.y * Percentage;
                break;
            case OverrideTypes.PercentageOfParentWidth:
                newSize.x = parentRT.sizeDelta.x * Percentage;
                break;
            case OverrideTypes.PercentageOfObjectHeight:
                newSize.x = objRT.sizeDelta.y * Percentage;
                break;
            case OverrideTypes.PercentageOfObjectWidth:
                newSize.x = objRT.sizeDelta.x * Percentage;
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
                newSize.y = parentRT.sizeDelta.y * Percentage;
                break;
            case OverrideTypes.PercentageOfParentWidth:
                newSize.y = parentRT.sizeDelta.x * Percentage;
                break;
            case OverrideTypes.PercentageOfObjectHeight:
                newSize.y = objRT.sizeDelta.y * Percentage;
                break;
            case OverrideTypes.PercentageOfObjectWidth:
                newSize.y = objRT.sizeDelta.x * Percentage;
                break;
            case OverrideTypes.Width:
                newSize.y = rt.rect.size.x;
                break;
        }

        rt.sizeDelta = newSize;
    }
	
	void Update () {
        ApplyOverride();
	}

#if UNITY_EDITOR
    void OnRenderObject()
    {
        ApplyOverride();
    }
#endif
}
