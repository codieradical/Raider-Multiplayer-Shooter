using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.Scene;
using UnityEngine.SceneManagement;

namespace Raider.Game.Audio
{

    /// <summary>
    /// I can expand this class to be a general music controller rather than just the mainmenu.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MainmenuMusicController : MonoBehaviour
    {
        void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        //Grab the audio source at the start.
        AudioSource audioSource;
        void Start()
        {
            DontDestroyOnLoad(this);

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        //Every time the a scene loads, check if it's a Ui scene.
        void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
        {
            if (Scenario.instance.currentGametype == Scenario.Gametype.Ui)
                audioSource.mute = false;
            else
                audioSource.mute = true;
        }
    }
}
