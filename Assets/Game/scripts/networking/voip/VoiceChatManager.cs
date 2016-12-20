using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using Raider.Game.Networking;

namespace Raider.Game.Networking.VoIP
{
    //This part contains all management functions.
    public partial class VoiceChatManager : MonoBehaviour
    {
#if UNITY_64
        const string INTERFACE_DLL_NAME = "TeamSpeakInterface64";
#elif UNITY_32
        const string INTERFACE_DLL_NAME = "TeamSpeakInterface32";
#else
        const string INTERFACE_DLL_NAME = "TeamSpeakInterface";
#endif
        static string SoundbackendsPath;

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "SetupLogging")]
        static extern void SetupLogging(Action<string> logCallback);

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "StartClient")]
        static extern bool StartClient(
            [MarshalAs(UnmanagedType.LPStr)] string username,
            [MarshalAs(UnmanagedType.LPStr)] string ipAddr, 
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string path, 
            ClientUIFunctions callbacks);

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "StopClient")]
        static extern bool StopClient();

        void OnClientDisconnect(int exitCode, string details)
        {
            Raider.Game.GUI.UserFeedback.LogError("VoIP ended with exit code " + exitCode.ToString());
            Raider.Game.GUI.UserFeedback.LogError(details);
            //ChatManager.
        }

        void TeamSpeakLogging(string message)
        {
            Debug.Log(message);
            //Raider.Game.GUI.UserFeedback.LogError(message);
        }

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            SoundbackendsPath = Application.dataPath + "/Game/scripts/voip/";
#else
            SoundbackendsPath = Application.dataPath + "/Plugins/";
#endif
            SetupLogging(TeamSpeakLogging);

            NetworkManager.instance.onNetworkStateClient += StartVoIPClient;
        }

        private void OnDestroy()
        {
            StopClient();
        }

        void StartVoIPClient()
        {
            ClientUIFunctions callbacks = new ClientUIFunctions();
            //username should be replaced with slot number.
            StartClient(Session.saveDataHandler.GetUsername(), NetworkManager.instance.networkAddress, 9987, SoundbackendsPath, callbacks);
        }
    }
}