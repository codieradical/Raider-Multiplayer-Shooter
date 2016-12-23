# define UNITY_64 //DEBUG: Raider Test environment doesn't define this automatically.
using UnityEngine;
using System.Runtime.InteropServices;
using System;
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

        bool serverRunning = false;
        bool clientRunning = false;


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
            if(serverRunning)
                StopServer();
            if(clientRunning)
                StopClient();
        }

        private void OnDisable()
        {
            if (serverRunning)
                StopServer();
            if (clientRunning)
                StopClient();
        }

        #region Client Functions

        void StartVoIPClient()
        {
            ClientUIFunctions callbacks = new ClientUIFunctions();
            //callbacks.onServerErrorEvent += OnServerErrorEvent;
            //callbacks.onServerStopEvent += OnServerStopEvent;
            //username should be replaced with slot number.
            StartClient(Session.saveDataHandler.GetUsername(), NetworkGameManager.instance.networkAddress, 9987, SoundbackendsPath, callbacks);
            clientRunning = true;
        }

        void StopVoIPClient()
        {
            StopClient();
            clientRunning = false;
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

            serverRunning = true;
        }

        void StopVoIPServer()
        {
            StopServer();
            serverRunning = false;
        }

#endregion

#region Client Callbacks

        void OnClientDisconnect(int exitCode, string details)
        {
            TeamSpeakLogging("VoIP ended with exit code " + exitCode.ToString());
            TeamSpeakLogging(details);
        }

        void OnServerErrorEvent(UInt64 serverConnectionHandlerID, string errorMessage, uint error, string returnCode, string extraMessage)
        {
            GUI.UserFeedback.LogError("A voice chat server error has occured. You have been disconnected.");
            StopVoIPClient();
        }

        void OnServerStopEvent(UInt64 serverConnectionHandlerID, string shutdownMessage)
        {
            GUI.UserFeedback.LogError("The voice chat server has unexpectedly shut down with the message: " + shutdownMessage.ToString());
            StopVoIPClient();
        }

        #endregion

        #region Server Callbacks

        #endregion
    }
}