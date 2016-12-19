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
        static extern bool StartClient(
            [MarshalAs(UnmanagedType.LPStr)] string username,
            [MarshalAs(UnmanagedType.LPStr)] string ipAddr, 
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string path, 
            ClientUIFunctions callbacks);

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

        void StartVoIPClient()
        {
            ClientUIFunctions callbacks = new ClientUIFunctions();
            //username should be replaced with slot number.
            StartClient(Session.saveDataHandler.GetUsername(), NetworkManager.instance.networkAddress, 9987, SoundbackendsPath, callbacks);
        }

        //This struct was taken from the TeamSpeak 3 C++ SDK and modified for C#.
        //Including source file would be very difficult.
        struct ClientUIFunctions
        {
            OnNewChannelEvent onNewChannelEvent;
            OnNewChannelCreatedEvent onNewChannelCreatedEvent;
            OnDelChannelEvent onDelChannelEvent;
            OnChannelMoveEvent onChannelMoveEvent;
            OnUpdateChannelEvent onUpdateChannelEvent;
            OnUpdateChannelEditedEvent onUpdateChannelEditedEvent;
            OnUpdateClientEvent onUpdateClientEvent;
            OnClientMoveEvent onClientMoveEvent;
            OnClientMoveSubscriptionEvent onClientMoveSubscriptionEvent;
            OnClientMoveTimeoutEvent onClientMoveTimeoutEvent;
            OnClientMoveMovedEvent onClientMoveMovedEvent;
            OnClientKickFromChannelEvent onClientKickFromChannelEvent;
            OnClientKickFromServerEvent onClientKickFromServerEvent;
            OnClientIDsEvent onClientIDsEvent;
            OnClientIDsFinishedEvent onClientIDsFinishedEvent;
            OnServerEditedEvent onServerEditedEvent;
            OnServerUpdatedEvent onServerUpdatedEvent;
            OnServerErrorEvent onServerErrorEvent;
            OnServerStopEvent onServerStopEvent;
            OnTextMessageEvent onTextMessageEvent;
            OnTalkStatusChangeEvent onTalkStatusChangeEvent;
            OnIgnoredWhisperEvent onIgnoredWhisperEvent;
            OnConnectionInfoEvent onConnectionInfoEvent;
            OnServerConnectionInfoEvent onServerConnectionInfoEvent;
            OnChannelSubscribeEvent onChannelSubscribeEvent;
            OnChannelSubscribeFinishedEvent onChannelSubscribeFinishedEvent;
            OnChannelUnsubscribeEvent onChannelUnsubscribeEvent;
            OnChannelUnsubscribeFinishedEvent onChannelUnsubscribeFinishedEvent;
            OnChannelDescriptionUpdateEvent onChannelDescriptionUpdateEvent;
            OnChannelPasswordChangedEvent onChannelPasswordChangedEvent;
            OnPlaybackShutdownCompleteEvent onPlaybackShutdownCompleteEvent;
            OnSoundDeviceListChangedEvent onSoundDeviceListChangedEvent;
            OnEditPlaybackVoiceDataEvent onEditPlaybackVoiceDataEvent;
            OnEditPostProcessVoiceDataEvent onEditPostProcessVoiceDataEvent;
            OnEditMixedPlaybackVoiceDataEvent onEditMixedPlaybackVoiceDataEvent;
            OnEditCapturedVoiceDataEvent onEditCapturedVoiceDataEvent;
            OnCustom3dRolloffCalculationClientEvent onCustom3dRolloffCalculationClientEvent;
            OnCustom3dRolloffCalculationWaveEvent onCustom3dRolloffCalculationWaveEvent;
            OnUserLoggingMessageEvent onUserLoggingMessageEvent;
            OnCustomPacketEncryptEvent onCustomPacketEncryptEvent;
            OnCustomPacketDecryptEvent onCustomPacketDecryptEvent;
            OnProvisioningSlotRequestResultEvent onProvisioningSlotRequestResultEvent;
            OnCheckServerUniqueIdentifierEvent onCheckServerUniqueIdentifierEvent;
            OnClientPasswordEncrypt onClientPasswordEncrypt;
            OnFileTransferStatusEvent onFileTransferStatusEvent;
            OnFileListEvent onFileListEvent;
            OnFileListFinishedEvent onFileListFinishedEvent;
            OnFileInfoEvent onFileInfoEvent;
            public delegate void OnNewChannelEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, UInt64 channelParentID);
            public delegate void OnNewChannelCreatedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, UInt64 channelParentID, Int16 invokerID, char[] invokerName, char[] invokerUniqueIdentifier);
            public delegate void OnDelChannelEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, Int16 invokerID, char[] invokerName, char[] invokerUniqueIdentifier);
            public delegate void OnChannelMoveEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, UInt64 newChannelParentID, Int16 invokerID, char[] invokerName, char[] invokerUniqueIdentifier);
            public delegate void OnUpdateChannelEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void OnUpdateChannelEditedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, Int16 invokerID, char[] invokerName, char[] invokerUniqueIdentifier);
            public delegate void OnUpdateClientEvent(UInt64 serverConnectionHandlerID, Int16 clientID, Int16 invokerID, char[] invokerName, char[] invokerUniqueIdentifier);
            public delegate void OnClientMoveEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, char[] moveMessage);
            public delegate void OnClientMoveSubscriptionEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility);
            public delegate void OnClientMoveTimeoutEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, char[] timeoutMessage);
            public delegate void OnClientMoveMovedEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, Int16 moverID, char[] moverName, char[] moverUniqueIdentifier, char[] moveMessage);
            public delegate void OnClientKickFromChannelEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, Int16 kickerID, char[] kickerName, char[] kickerUniqueIdentifier, char[] kickMessage);
            public delegate void OnClientKickFromServerEvent(UInt64 serverConnectionHandlerID, Int16 clientID, UInt64 oldChannelID, UInt64 newChannelID, int visibility, Int16 kickerID, char[] kickerName, char[] kickerUniqueIdentifier, char[] kickMessage);
            public delegate void OnClientIDsEvent(UInt64 serverConnectionHandlerID, char[] uniqueClientIdentifier, Int16 clientID, char[] clientName);
            public delegate void OnClientIDsFinishedEvent(UInt64 serverConnectionHandlerID);
            public delegate void OnServerEditedEvent(UInt64 serverConnectionHandlerID, Int16 editerID, char[] editerName, char[] editerUniqueIdentifier);
            public delegate void OnServerUpdatedEvent(UInt64 serverConnectionHandlerID);
            public delegate void OnServerErrorEvent(UInt64 serverConnectionHandlerID, char[] errorMessage, uint error, char[] returnCode, char[] extraMessage);
            public delegate void OnServerStopEvent(UInt64 serverConnectionHandlerID, char[] shutdownMessage);
            public delegate void OnTextMessageEvent(UInt64 serverConnectionHandlerID, Int16 targetMode, Int16 toID, Int16 fromID, char[] fromName, char[] fromUniqueIdentifier, char[] message);
            public delegate void OnTalkStatusChangeEvent(UInt64 serverConnectionHandlerID, int status, int isReceivedWhisper, Int16 clientID);
            public delegate void OnIgnoredWhisperEvent(UInt64 serverConnectionHandlerID, Int16 clientID);
            public delegate void OnConnectionInfoEvent(UInt64 serverConnectionHandlerID, Int16 clientID);
            public delegate void OnServerConnectionInfoEvent(UInt64 serverConnectionHandlerID);
            public delegate void OnChannelSubscribeEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void OnChannelSubscribeFinishedEvent(UInt64 serverConnectionHandlerID);
            public delegate void OnChannelUnsubscribeEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void OnChannelUnsubscribeFinishedEvent(UInt64 serverConnectionHandlerID);
            public delegate void OnChannelDescriptionUpdateEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void OnChannelPasswordChangedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID);
            public delegate void OnPlaybackShutdownCompleteEvent(UInt64 serverConnectionHandlerID);
            public delegate void OnSoundDeviceListChangedEvent(char[] modeID, int playOrCap);
            public delegate void OnEditPlaybackVoiceDataEvent(UInt64 serverConnectionHandlerID, Int16 clientID, short[] samples, int sampleCount, int channels);
            public delegate void OnEditPostProcessVoiceDataEvent(UInt64 serverConnectionHandlerID, Int16 clientID, short[] samples, int sampleCount, int channels, uint[] channelSpeakerArray, uint[] channelFillMask);
            public delegate void OnEditMixedPlaybackVoiceDataEvent(UInt64 serverConnectionHandlerID, short[] samples, int sampleCount, int channels, uint[] channelSpeakerArray, uint[] channelFillMask);
            public delegate void OnEditCapturedVoiceDataEvent(UInt64 serverConnectionHandlerID, short[] samples, int sampleCount, int channels, int[] edited);
            public delegate void OnCustom3dRolloffCalculationClientEvent(UInt64 serverConnectionHandlerID, Int16 clientID, float distance, float[] volume);
            public delegate void OnCustom3dRolloffCalculationWaveEvent(UInt64 serverConnectionHandlerID, UInt64 waveHandle, float distance, float[] volume);
            public delegate void OnUserLoggingMessageEvent(char[][] logmessage, int logLevel, char[] logChannel, UInt64 logID, char[] logTime, char[] completeLogString);
            public delegate void OnCustomPacketEncryptEvent(char[][] dataToSend, uint[] sizeOfData);
            public delegate void OnCustomPacketDecryptEvent(char[][] dataReceived, uint[] dataReceivedSize);
            public delegate void OnProvisioningSlotRequestResultEvent(uint error, UInt64 requestHandle, char[] connectionKey);
            public delegate void OnCheckServerUniqueIdentifierEvent(UInt64 serverConnectionHandlerID, char[] ServerUniqueIdentifier, int[] cancelConnect);
            public delegate void OnClientPasswordEncrypt(UInt64 serverConnectionHandlerID, char[] plaintext, string encryptedText, int encryptedTextByteSize);
            public delegate void OnFileTransferStatusEvent(Int16 transferID, uint status, char[] statusMessage, UInt64 remotefileSize, UInt64 serverConnectionHandlerID);
            public delegate void OnFileListEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, char[] path, char[] name, UInt64 size, UInt64 datetime, int type, UInt64 incompletesize, char[] returnCode);
            public delegate void OnFileListFinishedEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, char[] path);
            public delegate void OnFileInfoEvent(UInt64 serverConnectionHandlerID, UInt64 channelID, char[] name, UInt64 size, UInt64 datetime);
        } //END OF ClientUIFunctions
    }
}