using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Raider.Game.Scene
{

    public class Scenario : MonoBehaviour
    {

        [HideInInspector]
        public static Scenario instance;

        public string currentScene;
        public Gametype currentGametype = Gametype.None;

        public List<string> scenes;
        public List<string> getScenesByGametype(Gametype gametype)
        {
            List<string> appropriateScenes = new List<string>();

#if UNITY_EDITOR
            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                if (scene.path.Contains(gametype.ToString().ToLower()))
                    appropriateScenes.Add(scene.path.Remove(0, scene.path.LastIndexOf("/") + 1).Replace(".unity", ""));
            }
#else
            for(int i = SceneManager.sceneCount - 1; i > -1; i--)
            {
                if (SceneManager.GetSceneAt(i).path.Contains(gametype.ToString().ToLower()))
                    appropriateScenes.Add(SceneManager.GetSceneAt(i).name);
            }
#endif
            return appropriateScenes;
        }

        public enum Gametype
        {
            None,
            Slayer,
            Capture_The_Flag,
            King_Of_The_Hill,
            Assault,
            Oddball,
            Ui,
            Test

            //Excavator Gametypes:
            //Story,
            //Survival,
            //Seige,
            //Skirmish,
            //Anvil,
        }

        // Use this for initialization
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (instance != null)
                return;
            instance = this;

#if UNITY_EDITOR
            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                scenes.Add(scene.path.Remove(0, scene.path.LastIndexOf("/") + 1).Replace(".unity", ""));
            }
#else
            for(int i = SceneManager.sceneCount - 1; i > -1; i--)
            {
                scenes.Add(SceneManager.GetSceneAt(i).name);
            }
#endif

            currentScene = SceneManager.GetActiveScene().name;

        }

        void OnDestroy()
        {
            instance = null;
            Debug.LogWarning("Something just destroyed the scenario script!");
        }

        public void LoadScene(string sceneName, Gametype gametype)
        {
            SceneManager.LoadScene(sceneName);

            //Now the scene has loaded, we can check
#if !UNITY_EDITOR
            UnityEngine.SceneManagement.Scene newScene = SceneManager.GetSceneByName(sceneName);

            if (!newScene.path.Contains(gametype.ToString().ToLower()))
                Debug.LogError(string.Format("Incompatible Gametype, loaded {0} with gametype {1}", newScene.path + newScene.name, gametype.ToString()));
            
            SceneManager.SetActiveScene(newScene);
#endif
            currentGametype = gametype;

        }
    }
}