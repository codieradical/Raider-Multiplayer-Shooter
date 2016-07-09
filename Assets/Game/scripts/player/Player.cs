using UnityEngine;

[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour {

    public bool lockCursor = true;

	// Use this for initialization
	void Start () {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        this.gameObject.tag = "localPlayer";
    }
}
