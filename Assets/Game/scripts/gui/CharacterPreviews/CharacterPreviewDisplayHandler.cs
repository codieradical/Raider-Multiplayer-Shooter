using UnityEngine;
using System.Collections;

public class CharacterPreviewDisplayHandler : MonoBehaviour {

    public GameObject previewCharacterGraphics;

    float lastMouseX = -1;

    public void RotetePreview()
    {
        previewCharacterGraphics.transform.Rotate(Vector3.down * (Input.mousePosition.x - lastMouseX));
        UpdateLastX();
    }

    public void UpdateLastX()
    {
        lastMouseX = Input.mousePosition.x;
    }

    public void ZoomPreview()
    {

    }
}
