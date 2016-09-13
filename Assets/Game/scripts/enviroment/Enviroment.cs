using UnityEngine;
using System.Collections;
using System;

public class Enviroment : MonoBehaviour {

    //Upon implementation, add singleton.

    PhysicsSettings physicsSettings;

    [Serializable]
    public class PhysicsSettings
    {
        public Vector3 gravity = new Vector3(0, -9.81f, 0);
    }

	// Use this for initialization
	void Start () {
        Physics.gravity = physicsSettings.gravity;
    }
	
	// Update is called once per frame
	void OnValidate()
    {
        Physics.gravity = physicsSettings.gravity;
    }
}
