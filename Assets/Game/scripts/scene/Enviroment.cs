using System;
using UnityEngine;

namespace Raider.Game.Scene
{
    public class Enviroment : MonoBehaviour
    {

        public static Enviroment instance;

        void Awake()
        {
            if (instance != null)
                Debug.LogWarning("Enviroment singleton already instanced!");
            instance = this;
        }
        void OnDestroy()
        {
            instance = null;
        }

        public PhysicsSettings physicsSettings = new PhysicsSettings();

        [Serializable]
        public class PhysicsSettings
        {
            //
            public Vector3 gravity = new Vector3(0, -9.81f, 0);
            //lighting?
        }

        // Use this for initialization
        void Start()
        {
            Physics.gravity = physicsSettings.gravity;
        }

        // Update is called once per frame
        void OnValidate()
        {
            Physics.gravity = physicsSettings.gravity;
        }
    }
}