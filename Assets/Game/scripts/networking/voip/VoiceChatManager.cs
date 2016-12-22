using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using Raider.Game.Networking;
using Raider.Game.Player;

namespace Raider.Game.Networking.VoIP
{
    //This part contains all management functions.
    public partial class VoiceChatManager : MonoBehaviour
    {
        #if UNITY_EDITOR
            #if UNITY_64
                const string INTERFACE_DLL_NAME = "TeamSpeakInterface64_edi";
            #elif UNITY_32
                const string INTERFACE_DLL_NAME = "TeamSpeakInterface32_edi";
            #endif
        #elif !UNITY_EDITOR
            #if UNITY_64
                const string INTERFACE_DLL_NAME = "TeamSpeakInterface64";
            #elif UNITY_32
                const string INTERFACE_DLL_NAME = "TeamSpeakInterface32";
            #endif
        #endif

        static string SoundbackendsPath;

#region DLL Imports

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "SetupLogging")]
        static extern void SetupLogging(
            [MarshalAs(UnmanagedType.FunctionPtr)]Action<string> logCallback);

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "StartClient")]
        static extern bool StartClient(
            [MarshalAs(UnmanagedType.LPStr)] string username,
            [MarshalAs(UnmanagedType.LPStr)] string ipAddr, 
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string path, 
            ClientUIFunctions callbacks);

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "StopClient")]
        static extern bool StopClient();

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "StartServer")]
        static extern bool StartServer(
            [MarshalAs(UnmanagedType.LPStr)] string ipAddr,
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string serverName,
            ServerLibFunctions callbacks);

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "StartServer")]
        static extern bool StopServer();

#endregion

        void TeamSpeakLogging(string message)
        {
            Debug.Log(message);
            //Raider.Game.GUI.UserFeedback.LogError(message);
        }

        //Setup sound backends,
        //Setup logging,
        //Setup delegates.
        void Start()
        {
#if UNITY_EDITOR
                SoundbackendsPath = Application.dataPath + "/Game/scripts/voip/";
#else
                SoundbackendsPath = Application.dataPath + "/Plugins/";
#endif

            SetupLogging(TeamSpeakLogging);

            NetworkLobbyPlayerSetup.onLocalLobbyPlayerStart += StartVoIPClient;
            NetworkGameManager.instance.onClientDisconnect += StopVoIPClient;
            NetworkGameManager.instance.onStartServer += StartVoIPServer;
            NetworkGameManager.instance.onStopServer += StopVoIPServer;
        }

        //Make sure that teamspeak closes if the game is shut down.
        private void OnDestroy()
        {
            StopServer();
            StopClient();
        }

#region Client Functions

        void StartVoIPClient()
        {
            ClientUIFunctions callbacks = new ClientUIFunctions();
            //username should be replaced with slot number.
            StartClient(Session.saveDataHandler.GetUsername(), NetworkGameManager.instance.networkAddress, 9987, SoundbackendsPath, callbacks);
        }

        void StopVoIPClient()
        {
            StopClient();
        }

#endregion

#region Server Functions

        void StartVoIPServer()
        {
            ServerLibFunctions callbacks = new ServerLibFunctions();

            if(NetworkGameManager.instance.networkAddress == "localhost" || NetworkGameManager.instance.networkAddress == "127.0.0.1")
                StartServer("0.0.0.0", 9987, Session.saveDataHandler.GetUsername() + "'s Excavator VoIP Server", callbacks);
            else
                StartServer(NetworkGameManager.instance.networkAddress, 9987, Session.saveDataHandler.GetUsername() + "'s Excavator VoIP Server", callbacks);
        }

        void StopVoIPServer()
        {
            StopServer();
        }

#endregion

#region Client Callbacks

        void OnClientDisconnect(int exitCode, string details)
        {
            TeamSpeakLogging("VoIP ended with exit code " + exitCode.ToString());
            TeamSpeakLogging(details);
        }

#endregion

#region Server Callbacks

#endregion
    }
}