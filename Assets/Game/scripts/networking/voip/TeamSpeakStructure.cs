using System;
using UnityEngine;
using anyID = System.UInt16;

namespace Raider.Game.Networking.VoIP
{
	//This part containes TeamSpeak structs.
	//Referencing another class makes things complicated with the DLL imports.
	//And this is clean too.
	public partial class VoiceChatManager : MonoBehaviour
    {

        //This struct was taken from the TeamSpeak 3 C++ SDK and modified for C#.
        //Including source file would be very difficult.
        struct ClientUIFunctions
        {
            public OnNewChannelEvent onNewChannelEvent;
            public OnNewChannelCreatedEvent onNewChannelCreatedEvent;
            public OnDelChannelEvent onDelChannelEvent;
            public OnChannelMoveEvent onChannelMoveEvent;
            public OnUpdateChannelEvent onUpdateChannelEvent;
            public OnUpdateChannelEditedEvent onUpdateChannelEditedEvent;
            public OnUpdateClientEvent onUpdateClientEvent;
            public OnClientMoveEvent onClientMoveEvent;
            public OnClientMoveSubscriptionEvent onClientMoveSubscriptionEvent;
            public OnClientMoveTimeoutEvent onClientMoveTimeoutEvent;
            public OnClientMoveMovedEvent onClientMoveMovedEvent;
            public OnClientKickFromChannelEvent onClientKickFromChannelEvent;
            public OnClientKickFromServerEvent onClientKickFromServerEvent;
            public OnClientIDsEvent onClientIDsEvent;
            public OnClientIDsFinishedEvent onClientIDsFinishedEvent;
            public OnServerEditedEvent onServerEditedEvent;
            public OnServerUpdatedEvent onServerUpdatedEvent;
            public OnServerErrorEvent onServerErrorEvent;
            public OnServerStopEvent onServerStopEvent;
            public OnTextMessageEvent onTextMessageEvent;
            public OnTalkStatusChangeEvent onTalkStatusChangeEvent;
            public OnIgnoredWhisperEvent onIgnoredWhisperEvent;
            public OnConnectionInfoEvent onConnectionInfoEvent;
            public OnServerConnectionInfoEvent onServerConnectionInfoEvent;
            public OnChannelSubscribeEvent onChannelSubscribeEvent;
            public OnChannelSubscribeFinishedEvent onChannelSubscribeFinishedEvent;
            public OnChannelUnsubscribeEvent onChannelUnsubscribeEvent;
            public OnChannelUnsubscribeFinishedEvent onChannelUnsubscribeFinishedEvent;
            public OnChannelDescriptionUpdateEvent onChannelDescriptionUpdateEvent;
            public OnChannelPasswordChangedEvent onChannelPasswordChangedEvent;
            public OnPlaybackShutdownCompleteEvent onPlaybackShutdownCompleteEvent;
            public OnSoundDeviceListChangedEvent onSoundDeviceListChangedEvent;
            public OnEditPlaybackVoiceDataEvent onEditPlaybackVoiceDataEvent;
            public OnEditPostProcessVoiceDataEvent onEditPostProcessVoiceDataEvent;
            public OnEditMixedPlaybackVoiceDataEvent onEditMixedPlaybackVoiceDataEvent;
            public OnEditCapturedVoiceDataEvent onEditCapturedVoiceDataEvent;
            public OnCustom3dRolloffCalculationClientEvent onCustom3dRolloffCalculationClientEvent;
            public OnCustom3dRolloffCalculationWaveEvent onCustom3dRolloffCalculationWaveEvent;
            public OnUserLoggingMessageEvent onUserLoggingMessageEvent;
            public OnCustomPacketEncryptEvent onCustomPacketEncryptEvent;
            public OnCustomPacketDecryptEvent onCustomPacketDecryptEvent;
            public OnProvisioningSlotRequestResultEvent onProvisioningSlotRequestResultEvent;
            public OnCheckServerUniqueIdentifierEvent onCheckServerUniqueIdentifierEvent;
            public OnClientPasswordEncrypt onClientPasswordEncrypt;
            public OnFileTransferStatusEvent onFileTransferStatusEvent;
            public OnFileListEvent onFileListEvent;
            public OnFileListFinishedEvent onFileListFinishedEvent;
            public OnFileInfoEvent onFileInfoEvent;
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

        struct ServerLibFunctions
        {
            public OnVoiceDataEvent onVoiceDataEvent;
            public OnClientStartTalkingEvent onClientStartTalkingEvent;
            public OnClientStopTalkingEvent onClientStopTalkingEvent;
            public OnClientConnected onClientConnected;
            public OnClientDisconnected onClientDisconnected;
            public OnClientMoved onClientMoved;
            public OnChannelCreated onChannelCreated;
            public OnChannelEdited onChannelEdited;
            public OnChannelDeleted onChannelDeleted;
            public OnServerTextMessageEvent onServerTextMessageEvent;
            public OnChannelTextMessageEvent onChannelTextMessageEvent;
            public OnUserLoggingMessageEvent onUserLoggingMessageEvent;
            public OnAccountingErrorEvent onAccountingErrorEvent;
            public OnCustomPacketEncryptEvent onCustomPacketEncryptEvent;
            public OnCustomPacketDecryptEvent onCustomPacketDecryptEvent;
            public OnFileTransferEvent onFileTransferEvent;
            public PermClientCanConnect permClientCanConnect;
            public PermClientCanGetChannelDescription permClientCanGetChannelDescription;
            public PermClientUpdate permClientUpdate;
            public PermClientKickFromChannel permClientKickFromChannel;
            public PermClientKickFromServer permClientKickFromServer;
            public PermClientMove permClientMove;
            public PermChannelMove permChannelMove;
            public PermSendTextMessage permSendTextMessage;
            public PermServerRequestConnectionInfo permServerRequestConnectionInfo;
            public PermSendConnectionInfo permSendConnectionInfo;
            public PermChannelCreate permChannelCreate;
            public PermChannelEdit permChannelEdit;
            public PermChannelDelete permChannelDelete;
            public PermChannelSubscribe permChannelSubscribe;
            public PermFileTransferInitUpload permFileTransferInitUpload;
            public PermFileTransferInitDownload permFileTransferInitDownload;
            public PermFileTransferGetFileInfo permFileTransferGetFileInfo;
            public PermFileTransferGetFileList permFileTransferGetFileList;
            public PermFileTransferDeleteFile permFileTransferDeleteFile;
            public PermFileTransferCreateDirectory permFileTransferCreateDirectory;
            public PermFileTransferRenameFile permFileTransferRenameFile;
            public OnClientPasswordEncrypt onClientPasswordEncrypt;
            public OnTransformFilePath onTransformFilePath;
            public OnCustomServerPasswordCheck onCustomServerPasswordCheck;
            public OnCustomChannelPasswordCheck onCustomChannelPasswordCheck;

            public delegate void OnVoiceDataEvent(UInt64 serverID, anyID clientID, byte[] voiceData, uint voiceDataSize, uint frequency);
            public delegate void OnClientStartTalkingEvent(UInt64 serverID, anyID clientID);
            public delegate void OnClientStopTalkingEvent(UInt64 serverID, anyID clientID);
            public delegate void OnClientConnected(UInt64 serverID, anyID clientID, UInt64 channelID, uint[] removeClientError);
            public delegate void OnClientDisconnected(UInt64 serverID, anyID clientID, UInt64 channelID);
            public delegate void OnClientMoved(UInt64 serverID, anyID clientID, UInt64 oldChannelID, UInt64 newChannelID);
            public delegate void OnChannelCreated(UInt64 serverID, anyID invokerClientID, UInt64 channelID);
            public delegate void OnChannelEdited(UInt64 serverID, anyID invokerClientID, UInt64 channelID);
            public delegate void OnChannelDeleted(UInt64 serverID, anyID invokerClientID, UInt64 channelID);
            public delegate void OnServerTextMessageEvent(UInt64 serverID, anyID invokerClientID, char[] textMessage);
            public delegate void OnChannelTextMessageEvent(UInt64 serverID, anyID invokerClientID, UInt64 targetChannelID, char[] textMessage);
            public delegate void OnUserLoggingMessageEvent(char[] logmessage, int logLevel, char[] logChannel, UInt64 logID, char[] logTime, char[] completeLogString);
            public delegate void OnAccountingErrorEvent(UInt64 serverID, uint errorCode);
            public delegate void OnCustomPacketEncryptEvent(char[][] dataToSend, uint[] sizeOfData);
            public delegate void OnCustomPacketDecryptEvent(char[][] dataReceived, uint[] dataReceivedSize);
            public delegate void OnFileTransferEvent(FileTransferCallbackExport[] data);
            public delegate uint PermClientCanConnect(UInt64 serverID, ClientMiniExport[] client);
            public delegate uint PermClientCanGetChannelDescription(UInt64 serverID, ClientMiniExport[] client);
            public delegate uint PermClientUpdate(UInt64 serverID, anyID clientID, VariablesExport[] variables);
            public delegate uint PermClientKickFromChannel(UInt64 serverID, ClientMiniExport[] client, int toKickCount, ClientMiniExport[] toKickClients, char[] reasonText);
            public delegate uint PermClientKickFromServer(UInt64 serverID, ClientMiniExport[] client, int toKickCount, ClientMiniExport[] toKickClients, char[] reasonText);
            public delegate uint PermClientMove(UInt64 serverID, ClientMiniExport[] client, int toMoveCount, ClientMiniExport[] toMoveClients, UInt64 newChannel, char[] reasonText);
            public delegate uint PermChannelMove(UInt64 serverID, ClientMiniExport[] client, UInt64 channelID, UInt64 newParentChannelID);
            public delegate uint PermSendTextMessage(UInt64 serverID, ClientMiniExport[] client, anyID targetMode, UInt64 targetClientOrChannel, char[] textMessage);
            public delegate uint PermServerRequestConnectionInfo(UInt64 serverID, ClientMiniExport[] client);
            public delegate uint PermSendConnectionInfo(UInt64 serverID, ClientMiniExport[] client, int[] mayViewIpPort, ClientMiniExport[] targetClient);
            public delegate uint PermChannelCreate(UInt64 serverID, ClientMiniExport[] client, UInt64 parentChannelID, VariablesExport[] variables);
            public delegate uint PermChannelEdit(UInt64 serverID, ClientMiniExport[] client, UInt64 channelID, VariablesExport[] variables);
            public delegate uint PermChannelDelete(UInt64 serverID, ClientMiniExport[] client, UInt64 channelID);
            public delegate uint PermChannelSubscribe(UInt64 serverID, ClientMiniExport[] client, UInt64 channelID);
            public delegate uint PermFileTransferInitUpload(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftinitupload[] _params);
            public delegate uint PermFileTransferInitDownload(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftinitdownload[] _params);
            public delegate uint PermFileTransferGetFileInfo(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftgetfileinfo[] _params);
            public delegate uint PermFileTransferGetFileList(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftgetfilelist[] _params);
            public delegate uint PermFileTransferDeleteFile(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftdeletefile[] _params);
            public delegate uint PermFileTransferCreateDirectory(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftcreatedir[] _params);
            public delegate uint PermFileTransferRenameFile(UInt64 serverID, ClientMiniExport[] client, Ts3sc_ftrenamefile[] _params);
            public delegate void OnClientPasswordEncrypt(UInt64 serverID, char[] plaintext, char[] encryptedText, int encryptedTextByteSize);
            public delegate uint OnTransformFilePath(UInt64 serverID, anyID invokerClientID, TransformFilePathExport[] original, TransformFilePathExportReturns[] result);
            public delegate uint OnCustomServerPasswordCheck(UInt64 serverID, ClientMiniExport[] client, char[] password);
            public delegate uint OnCustomChannelPasswordCheck(UInt64 serverID, ClientMiniExport[] client, UInt64 channelID, char[] password);

        } //END OF ServerLibFunctions

        #region serverlib structs

        struct VariablesExportItem
        {
            char itemIsValid;    /* This item has valid values. ignore this item if 0 */
            char proposedIsSet;  /* The value in proposed is set. if 0 ignore proposed */
            char[] current;          /* current value (stored in memory) */
            char[] proposed;         /* New value to change to (const, so no updates please) */
        };

        struct VariablesExport
        {
            VariablesExportItem[] items;
        };

        struct ClientMiniExport
        {
            anyID ID;
            UInt64 channel;
            char[] ident;
            char[] nickname;
        };

        struct TransformFilePathExport
        {
            UInt64 channel;
            char[] filename;
            int action;
            int transformedFileNameMaxSize;
            int channelPathMaxSize;
        };

        struct TransformFilePathExportReturns
        {
            char[] transformedFileName;
            char[] channelPath;
            int logFileAction;
        };

        struct FileTransferCallbackExport
        {
            anyID clientID;
            anyID transferID;
            anyID remoteTransferID;
            uint status;
            char[] statusMessage;
            UInt64 remotefileSize;
            UInt64 bytes;
            int isSender;
        };

        #endregion

        #region server command structs

        /** Structure that contains the command data for an ftcreatedir message */
        struct Ts3sc_data_ftcreatedir
        {
            UInt64 channelID; /**< The channel ID where the file is to be uploaded*/
            char[] dirname;   /**< The directory name*/
        }

        /** Structure that contains the meta data for an ftcreatedir message */
        struct Ts3sc_meta_ftcreatedir
        {
            int RESERVED; /**<This is here because C forbids empty structs. DO NOT USE*/
        }

        /** C Structure that contains the meta data and data for an ftcreatedir message */
        struct Ts3sc_ftcreatedir
        {
            Ts3sc_meta_ftcreatedir m;      /**< message meta data*/
            Ts3sc_data_ftcreatedir d;      /**< message data*/
        }

        /** Structure that contains the command data for an ftdeletefile message */
        struct Ts3sc_data_ftdeletefile
        {
            UInt64 channelID; /**< The channel ID where the file is to be deleted*/
        }

        /** Structure that contains the repeat command data for an ftdeletefile message */
        struct Ts3sc_array_ftdeletefile
        {
            char[] fileName;  /**< The file name to be deleted*/
        }

        /** Structure that contains the meta data for an ftdeletefile message */
        struct Ts3sc_meta_ftdeletefile
        {
            int RESERVED; /**<This is here because C forbids empty structs. DO NOT USE*/
        }

        /** C Structure that contains the meta data and data for an ftdeletefile message */
        struct Ts3sc_ftdeletefile
        {
            Ts3sc_meta_ftdeletefile m;     /**< message meta data*/
            Ts3sc_data_ftdeletefile d;     /**< message data*/
            int r_size;                       /**< items in r*/
            Ts3sc_array_ftdeletefile[] r;     /**< message repeat data*/
        }

        /** Structure that contains the command data for an ftgetfileinfo message */
        struct Ts3sc_data_ftgetfileinfo
        {
            int RESERVED; /**< This is here because C forbids empty structs. DO NOT USE*/
        }

        /** Structure that contains the repeat command data for an ftgetfileinfo message */
        struct Ts3sc_array_ftgetfileinfo
        {
            UInt64 channelID; /**< The channel ID where the file is located*/
            char[] fileName; /**< The file name*/
        }

        /** Structure that contains the meta data for an ftgetfileinfo message */
        struct Ts3sc_meta_ftgetfileinfo
        {
            int RESERVED; /**<This is here because C forbids empty structs. DO NOT USE*/
        }

        /** C Structure that contains the meta data and data for an ftgetfileinfo message */
        struct Ts3sc_ftgetfileinfo
        {
            Ts3sc_meta_ftgetfileinfo m;      /**< message meta data*/
            Ts3sc_data_ftgetfileinfo d;      /**< message data*/
            int r_size; /**< items in r*/
            Ts3sc_array_ftgetfileinfo[] r;      /**< message repeat data*/
        }

        /** Structure that contains the command data for an ftgetfilelist message */
        struct Ts3sc_data_ftgetfilelist
        {
            UInt64 channelID; /**< The channel ID*/
            char[] path;      /**< The path where to get the list for*/
        }

        /** Structure that contains the meta data for an ftgetfilelist message */
        struct Ts3sc_meta_ftgetfilelist
        {
            int RESERVED; /**<This is here because C forbids empty structs. DO NOT USE*/
        }

        /** C Structure that contains the meta data and data for an ftgetfilelist message */
        struct Ts3sc_ftgetfilelist
        {
            Ts3sc_meta_ftgetfilelist m;      /**< message meta data*/
            Ts3sc_data_ftgetfilelist d;      /**< message data*/
        }

        /** Structure that contains the command data for an ftinitdownload message */
        struct Ts3sc_data_ftinitdownload
        {
            char[] fileName;  /**< The file name*/
            UInt64 channelID; /**< The channel ID where the file is to be downloaded from*/
        }

        /** Structure that contains the meta data for an ftinitdownload message */
        struct Ts3sc_meta_ftinitdownload
        {
            int RESERVED; /**<This is here because C forbids empty structs. DO NOT USE*/
        }

        /** C Structure that contains the meta data and data for an ftinitdownload message */
        struct Ts3sc_ftinitdownload
        {
            Ts3sc_meta_ftinitdownload m;      /**< message meta data*/
            Ts3sc_data_ftinitdownload d;      /**< message data*/
        };

        /** Structure that contains the command data for an ftinitupload message */
        struct Ts3sc_data_ftinitupload
        {
            char[] fileName;  /**< The file name*/
            UInt64 fileSize;  /**< The file size*/
            UInt64 channelID; /**< The channel ID where the file is to be uploaded*/
            int overwrite; /**< Set to 1 to overwrite files, 0 to prevent overwrites */
            int resume;    /**< Set to 1 to resume an existing upload, 0 to start new*/
        }

        /** Structure that contains the meta data for an ftinitupload message */
        struct Ts3sc_meta_ftinitupload
        {
            int RESERVED; /**<This is here because C forbids empty structs. DO NOT USE*/
        }

        /** C Structure that contains the meta data and data for an ftinitupload message */
        struct Ts3sc_ftinitupload
        {
            Ts3sc_meta_ftinitupload m;      /**< message meta data*/
            Ts3sc_data_ftinitupload d;      /**< message data*/
        };

        /** Structure that contains the command data for an ftrenamefile message */
        struct Ts3sc_data_ftrenamefile
        {
            UInt64 fromChannelID; /**< The channel ID where the file is located now*/
            UInt64 toChannelID;   /**< The channel ID where the file is to be moved to*/
            char[] oldFileName;   /**< The current file name*/
            char[] newFileName;   /**< The new file name*/
        };

        /** Structure that contains the meta data for an ftrenamefile message */
        struct Ts3sc_meta_ftrenamefile
        {
            uint has_toChannelID; /**< set to 1 if this message had tcid defined */
        };

        /** C Structure that contains the meta data and data for an ftrenamefile message */
        struct Ts3sc_ftrenamefile
        {
            Ts3sc_meta_ftrenamefile m;      /**< message meta data*/
            Ts3sc_data_ftrenamefile d;      /**< message data*/
        }
    }
#endregion

}