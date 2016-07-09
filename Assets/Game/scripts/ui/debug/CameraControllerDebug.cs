using UnityEngine;
using System.Collections;

public class CameraControllerDebug : MonoBehaviour {

	// Use this for initialization
	void OnGUI()
    {
        string selectedCameraMode = GameObject.Find("CameraPoint").GetComponent<CameraModeController>().selectedCameraMode.ToString();

        Rect labelRect = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(labelRect, string.Format(
            "GAME\n" +
            "test: Camera Controllers\n" +
            "DO NOT REDISTRIBUTE\n\n" +
            "Selected Camera Mode: {0}\n" +
            "Press F to switch.\n" +
            "Scrolling changes camera distance (where appropriate)." 
            , selectedCameraMode));
    }
}
