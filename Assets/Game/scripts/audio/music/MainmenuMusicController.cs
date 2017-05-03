using Raider.Game.Scene;
using UnityEngine;
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
            //Add the OnActiveSceneChanged delegate to the scenemanager activescenechanged event.
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        //Grab the audio source at the start.
        AudioSource audioSource;
        void Start()
        {
            //Make sure this object remains active inbetween scenes.
            DontDestroyOnLoad(this);

            //If there's no audiosource assigned in the editor, look for one.
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        //Every time the a scene loads, check if it's a Ui scene.
        void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
        {
            //If there's no audiosource assigned, look for one.
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            //If the current gametype is Ui, unmute the mainmenu music.
            if (Scenario.instance.currentGametype == Gametypes.GametypeHelper.Gametype.Ui)
                audioSource.mute = false;
            //Else mute it.
            else
                audioSource.mute = true;
        }
    }
}
