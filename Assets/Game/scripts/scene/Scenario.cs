using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Raider.Game.Networking;
using System.Collections;

namespace Raider.Game.Scene
{

    public class Scenario : MonoBehaviour
    {

        [HideInInspector]
        public static Scenario instance;

        public string gameuiScene;
        public string currentScene;
        public Gametype currentGametype = Gametype.None;

        public static bool InLobby
        {
            get
            {
                if (SceneManager.GetActiveScene().name == NetworkManager.instance.lobbyScene)
                    return true;
                else
                    return false;
            }
        }

        public void LeaveGame()
        {
            if(InLobby)
            {
                Debug.LogWarning("Can't leave game when in lobby!");
            }
            else
            {
                if(NetworkManager.instance.CurrentNetworkState != NetworkManager.NetworkState.Offline)
                {
                    if (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Host || NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server)
                        NetworkManager.instance.ServerReturnToLobby();
                    else
                        NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
                }
                LoadScene(NetworkManager.instance.lobbyScene, Gametype.Ui);
            }
        }

        //A list of scenes by path.
        public List<string> scenes;

        public List<string> GetSceneNamesByGametype(Gametype gametype)
        {
            List<string> paths = GetScenePathsByGametype(gametype);
            List<string> names = new List<string>();

            foreach (string path in paths)
            {
                names.Add(path.Remove(0, path.LastIndexOf("/") + 1).Replace(".unity", ""));
            }

            return names;
        }

        public List<string> GetScenePathsByGametype(Gametype gametype)
        {
            List<string> appropriateScenes = new List<string>();

#if UNITY_EDITOR
            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                if (scene.path.Contains(gametype.ToString().ToLower()))
                    appropriateScenes.Add(scene.path.Remove(0, scene.path.LastIndexOf("/") + 1).Replace(".unity", ""));
            }
#else
            foreach(string scene in scenes)
            {
                if (scene.Contains(gametype.ToString().ToLower()))
                    appropriateScenes.Add(scene);
            }
#endif
            return appropriateScenes;
        }

        public enum Gametype
        {
            //1 represents a game scene, 2 represents a lobby scene.
            None = 0,
            Slayer = 1,
            Capture_The_Flag = 1,
            King_Of_The_Hill = 1,
            Assault = 1,
            Oddball = 1,
            Ui = 2,
            Test = 1

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
                scenes.Add(scene.path);
            }
#else
            for(int i = SceneManager.sceneCountInBuildSettings - 1; i > -1; i--)
            {
                scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
            }
#endif

            currentScene = SceneManager.GetActiveScene().name;

        }

        void OnDestroy()
        {
            instance = null;
            Debug.LogWarning("Something just destroyed the scenario script!");
        }

        public void NetworkLoadedScene()
        {
            currentScene = NetworkManager.networkSceneName;

            if ((int)currentGametype == 1)
                StartCoroutine(LoadGameUI());
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
            currentScene = sceneName;
            currentGametype = gametype;

            //If this is a game scene, load the game UI scene additive.
            if ((int)gametype == 1)
                StartCoroutine(LoadGameUI());

        }

        //This is a kinda hacky solution to merging the next frame.
        IEnumerator LoadGameUI()
        {
            //Load the GameUI scene.
            SceneManager.LoadScene(gameuiScene, LoadSceneMode.Additive);

            //Wait a frame...
            yield return 0;

            //Merge the GameUI scene into the Game scene, destroy the GameUI scene.
            //This prevents on active scene changed being called too much.
            SceneManager.MergeScenes(SceneManager.GetSceneByName(gameuiScene), SceneManager.GetSceneByName(currentScene));
        }

        public static Sprite GetMapImage(string mapName)
        {
            //Load singular just wouldn't work!
            Sprite image = Resources.Load<Sprite>("gui/mapImg/" + mapName);
            if (image == null) //If no image was found, load the template.
                return Resources.Load<Sprite>("gui/mapImg/template");
            else
                return image;
        }
    }
}