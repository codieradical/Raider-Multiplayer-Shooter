using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Layout/Cell Size Override")]
[ExecuteInEditMode]
[RequireComponent(typeof(GridLayoutGroup))]
public class CellSizeOverride : MonoBehaviour {

    public GameObject providedGameObject;
    public float Percentage;

    public enum OverrideTypes
    {
        None,
        PercentageOfWidth,
        PercentageOfHeight,
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
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        RectTransform parentRT = transform.parent.GetComponent<RectTransform>();
        RectTransform objRT = null;
        if(providedGameObject != null)
            objRT = providedGameObject.GetComponent<RectTransform>();
        Vector2 newSize = new Vector2(gridLayout.cellSize.x, gridLayout.cellSize.y);

        switch(widthOverride)
        {
            case OverrideTypes.None:
                break;
            case OverrideTypes.PercentageOfParentHeight:
                newSize.x = parentRT.rect.size.y * Percentage;
                break;
            case OverrideTypes.PercentageOfParentWidth:
                newSize.x = parentRT.rect.size.x * Percentage;
                break;
            case OverrideTypes.PercentageOfHeight:
                newSize.x = rt.rect.size.y * Percentage;
                break;
            case OverrideTypes.PercentageOfWidth:
                newSize.x = rt.rect.size.x * Percentage;
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
            case OverrideTypes.PercentageOfHeight:
                newSize.y = rt.rect.size.y * Percentage;
                break;
            case OverrideTypes.PercentageOfWidth:
                newSize.y = rt.rect.size.x * Percentage;
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

        //If the number is nagative, reverse it, since a cell can't be negative.
        if (newSize.x < 0)
            newSize.x *= -1;
        if (newSize.y < 0)
            newSize.y *= -1;

        gridLayout.cellSize = newSize;
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
