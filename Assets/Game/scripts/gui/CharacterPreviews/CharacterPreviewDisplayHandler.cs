using UnityEngine;
using System.Collections;

public class CharacterPreviewDisplayHandler : MonoBehaviour {

    private GameObject previewCharacterGraphics;

	// Use this for initialization
    // Model previews are instanced during runtime,
    // the preview model generator needs to handle this.
	void AssignPreviewModel(GameObject previewCharacter)
    {
        previewCharacterGraphics = previewCharacter.transform.Find("Graphics").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
