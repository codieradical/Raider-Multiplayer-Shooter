using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using Raider.Game.Networking;

namespace Raider.Game.Networking.VoIP
{

    public class VoiceChatManager : MonoBehaviour
    {
        const string INTERFACE_DLL_NAME = "TeamSpeak Interface";
        static string SoundbackendsPath;

        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode)]
        static extern void SetupLogging(Action<string> logCallback);
        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode)]
        static extern bool StartClient(char[] username, char[] ipAddr, int port, ClientUIFunctions callbacks, char[] path);
        [DllImport(INTERFACE_DLL_NAME, CharSet = CharSet.Unicode)]
        static extern bool StopClient();

        void OnClientDisconnect(int exitCode, string details)
        {
            Raider.Game.GUI.UserFeedback.LogError("VoIP ended with exit code " + exitCode.ToString());
            Raider.Game.GUI.UserFeedback.LogError(details);
            //ChatManager.
        }

        void TeamSpeakLogging(string message)
        {
            Raider.Game.GUI.UserFeedback.LogError(message);
        }

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            SoundbackendsPath = Application.dataPath + "/Game/scripts/voip";
#else
            SoundbackendsPath = Application.dataPath + "/VoIP";
#endif
            Debug.LogError(SoundbackendsPath);
            ClientUIFunctions callbacks = new ClientUIFunctions();
            SetupLogging(TeamSpeakLogging);
            StartClient("Alex231".ToCharArray(), "localhost".ToCharArray(), 9987, callbacks, SoundbackendsPath.ToCharArray());
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        //This struct was taken from the TeamSpeak 3 C++ SDK and modified for C#.
        //Including source file would be very difficult.
        struct ClientUIFunctions
        {
            public delegate void onNewChannelEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, UInt64 channelParentID);
            public delegate void onNewChannelCreatedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, UInt64 channelParentID, Int16 invokerID, string invokerName, string invokerUniqueIdentifier);
            public delegate void onDelChannelEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, Int16 invokerID, string invokerName, string invokerUniqueIdentifier);
            public delegate void onChannelMoveEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, UInt64 newChannelParentID, Int16 invokerID, string invokerName, string invokerUniqueIdentifier);
            public delegate void onUpdateChannelEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void onUpdateChannelEditedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, Int16 invokerID, string invokerName, string invokerUniqueIdentifier);
            public delegate void onUpdateClientEvent(UInt64 serverConnectionHandlerID, Int16 clientID, Int16 invokerID, string invokerName, string invokerUniqueIdentifier);
            public delegate void onClientMoveEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, string moveMessage);
            public delegate void onClientMoveSubscriptionEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility);
            public delegate void onClientMoveTimeoutEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, string timeoutMessage);
            public delegate void onClientMoveMovedEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, Int16 moverID, string moverName, string moverUniqueIdentifier, string moveMessage);
            public delegate void onClientKickFromChannelEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, Int16 kickerID, string kickerName, string kickerUniqueIdentifier, string kickMessage);
            public delegate void onClientKickFromServerEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, Int16 kickerID, string kickerName, string kickerUniqueIdentifier, string kickMessage);
            public delegate void onClientIDsEvent(UInt64 serverConnectionHandlerID, string uniqueClientIdentifier, Int16 clientID, string clientName);
            public delegate void onClientIDsFinishedEvent(UInt64 serverConnectionHandlerID);
            public delegate void onServerEditedEvent(UInt64 serverConnectionHandlerID, Int16 editerID, string editerName, string editerUniqueIdentifier);
            public delegate void onServerUpdatedEvent(UInt64 serverConnectionHandlerID);
            public delegate void onServerErrorEvent(UInt64 serverConnectionHandlerID, string errorMessage, uint error, string returnCode, string extraMessage);
            public delegate void onServerStopEvent(UInt64 serverConnectionHandlerID, string shutdownMessage);
            public delegate void onTextMessageEvent(UInt64 serverConnectionHandlerID, Int16 targetMode, Int16 toID, Int16 fromID, string fromName, string fromUniqueIdentifier, string message);
            public delegate void onTalkStatusChangeEvent(UInt64 serverConnectionHandlerID, int status, int isReceivedWhisper, Int16 clientID);
            public delegate void onIgnoredWhisperEvent(UInt64 serverConnectionHandlerID, Int16 clientID);
            public delegate void onConnectionInfoEvent(UInt64 serverConnectionHandlerID, Int16 clientID);
            public delegate void onServerConnectionInfoEvent(UInt64 serverConnectionHandlerID);
            public delegate void onChannelSubscribeEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void onChannelSubscribeFinishedEvent(UInt64 serverConnectionHandlerID);
            public delegate void onChannelUnsubscribeEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void onChannelUnsubscribeFinishedEvent(UInt64 serverConnectionHandlerID);
            public delegate void onChannelDescriptionUpdateEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void onChannelPasswordChangedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void onPlaybackShutdownCompleteEvent(UInt64 serverConnectionHandlerID);
            public delegate void onSoundDeviceListChangedEvent(string modeID, int playOrCap);
            public delegate void onEditPlaybackVoiceDataEvent(UInt64 serverConnectionHandlerID, Int16 clientID, short[] samples, int sampleCount, int channels);
            public delegate void onEditPostProcessVoiceDataEvent(UInt64 serverConnectionHandlerID, Int16 clientID, short[] samples, int sampleCount, int channels, uint[] channelSpeakerArray, uint[] channelFillMask);
            public delegate void onEditMixedPlaybackVoiceDataEvent(UInt64 serverConnectionHandlerID, short[] samples, int sampleCount, int channels, uint[] channelSpeakerArray, uint[] channelFillMask);
            public delegate void onEditCapturedVoiceDataEvent(UInt64 serverConnectionHandlerID, short[] samples, int sampleCount, int channels, int[] edited);
            public delegate void onCustom3dRolloffCalculationClientEvent(UInt64 serverConnectionHandlerID, Int16 clientID, float distance, float[] volume);
            public delegate void onCustom3dRolloffCalculationWaveEvent(UInt64 serverConnectionHandlerID, UInt64 waveHandle, float distance, float[] volume);
            public delegate void onUserLoggingMessageEvent(string logmessage, int logLevel, string logChannel, UInt64 logID, string logTime, string completeLogString);
            public delegate void onCustomPacketEncryptEvent(string[] dataToSend, uint[] sizeOfData);
            public delegate void onCustomPacketDecryptEvent(string[] dataReceived, uint[] dataReceivedSize);
            public delegate void onProvisioningSlotRequestResultEvent(uint error, UInt64 requestHandle, string connectionKey);
            public delegate void onCheckServerUniqueIdentifierEvent(UInt64 serverConnectionHandlerID, string ServerUniqueIdentifier, int[] cancelConnect);
            public delegate void onClientPasswordEncrypt(UInt64 serverConnectionHandlerID, string plaintext, string encryptedText, int encryptedTextByteSize);
            public delegate void onFileTransferStatusEvent(Int16 transferID, uint status, string statusMessage, UInt64 remotefileSize, UInt64 serverConnectionHandlerID);
            public delegate void onFileListEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, string path, string name, UInt64 size, UInt64 datetime, int type, UInt64 incompletesize, string returnCode);
            public delegate void onFileListFinishedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, string path);
            public delegate void onFileInfoEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, string name, UInt64 size, UInt64 datetime);
        } //END OF ClientUIFunctions
    }
}