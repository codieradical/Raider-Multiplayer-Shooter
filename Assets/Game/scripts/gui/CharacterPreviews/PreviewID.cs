using UnityEngine;
using System.Collections;

public class PreviewID : MonoBehaviour {

    //The username of the player who owns this preview.
    public string player;
    //The character slot of the preview.
    public int slot;
    
    public CharacterPreviewHandler.PreviewType previewType;
    public SaveDataStructure.Character appearenceCharacter;
    public GameObject graphicsObject;
    public Camera camera;

    public Texture previewTexture;
}
