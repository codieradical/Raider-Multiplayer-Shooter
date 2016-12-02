using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.Scene;


namespace Raider.Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MainmenuMusicController : MonoBehaviour
    {
        AudioSource audioSource;
        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            //This game object starts on every scene it loads on.
            //So the gametype will change.
            if (Scenario.instance.currentGametype == Scenario.Gametype.Ui)
                audioSource.mute = false;
            else
                audioSource.mute = true;
        }
    }
}
