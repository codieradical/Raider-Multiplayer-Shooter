using Raider.Game.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Networking
{
	[DisallowMultipleComponent]
    public class NetworkLANDiscovery : MonoBehaviour
    {
        string nextBroadcast;
        public string BroadcastData
        {
            set
            {
                if(isServer && NetworkServer.active && running)
                {
                    nextBroadcast = value;

                    if (restartServer != null)
                        StopCoroutine(restartServer);

                    restartServer =  StartCoroutine(RestartServer(value));
                }
            }
        }

        Coroutine restartServer;

        IEnumerator RestartServer(string value)
        {
            if (running)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                StopBroadcast();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }
            broadcastData = value;
            yield return new WaitForEndOfFrame();
            Debug.Log("Restarting discovery server.");
            Initialize();
            StartAsServer();
        }

        public List<NetworkLobbyBroadcastData> DiscoveredLobbies
        {
            get
            {
                List<NetworkLobbyBroadcastData> lobbies = new List<NetworkLobbyBroadcastData>();
                if (broadcastsReceived == null)
                    return lobbies;

                foreach (KeyValuePair<string, NetworkBroadcastResult> broadcast in broadcastsReceived)
                    lobbies.Add(NetworkLobbyBroadcastData.FromJSONString(BytesToString(broadcast.Value.broadcastData)));
                return lobbies;
            }
        }

        string lastBroadcast;

        void UpdateBroadcastData()
        {
            if (Scenario.InLobby && NetworkServer.active && NetworkLobbyBroadcastData.CurrentLobbyData().JSONString != lastBroadcast && NetworkLobbyBroadcastData.CurrentLobbyData().JSONString != nextBroadcast)
            {
                BroadcastData = NetworkLobbyBroadcastData.CurrentLobbyData().JSONString;
            }

            lastBroadcast = broadcastData;
        }

        protected virtual void OnReceivedBroadcast(string fromAddress, string data)
        {
            //base.OnReceivedBroadcast(fromAddress, data);
            Debug.Log("Got broadcast from [" + fromAddress + "] " + data);
        }

        /*The code below is taken from Unity 5.6's Networking source code repository on BitBucket.
         * Retrieved from https://bitbucket.org/Unity-Technologies/networking/src/bfdfc58bb61bfdd7d49ceb8c69583482febadc84/Runtime/NetworkDiscovery.cs?at=5.6&fileviewer=file-view-default
         * Retreived on 13/5/17
         * 
         * I may have been able to inherit from NetworkDiscovery,
         * But there would be bloat, and I was having issues with internal methods.
         * 
         * The code has been modified significantly:
         * - All properties have been removed, as they are not required.
         * - GUI implementation removed.
         * - NetworkManager implementation removed.
         */

        #region Unity Engine NetworkDiscovery (Modified)

        const int k_MaxBroadcastMsgSize = 1024;

        // config data
        [SerializeField]
        int broadcastPort = 47777;

        [SerializeField]
        int broadcastKey = 2222;

        [SerializeField]
        int broadcastVersion = 1;

        [SerializeField]
        int broadcastSubVersion = 1;

        [SerializeField]
        int broadcastInterval = 1000;

        [SerializeField]
        string broadcastData = "HELLO";

        // runtime data
        int hostId = -1;
        bool running;

        bool isServer;
        bool isClient;

        byte[] msgOutBuffer;
        byte[] msgInBuffer;
        HostTopology defaultTopology;
        Dictionary<string, NetworkBroadcastResult> broadcastsReceived;

        static byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public bool Initialize()
        {
            if (broadcastData.Length >= k_MaxBroadcastMsgSize)
            {
                if (LogFilter.logError) { Debug.LogError("NetworkDiscovery Initialize - data too large. max is " + k_MaxBroadcastMsgSize); }
                return false;
            }

            if (!NetworkTransport.IsStarted)
            {
                NetworkTransport.Init();
            }

            msgOutBuffer = StringToBytes(broadcastData);
            msgInBuffer = new byte[k_MaxBroadcastMsgSize];
            broadcastsReceived = new Dictionary<string, NetworkBroadcastResult>();

            ConnectionConfig cc = new ConnectionConfig();
            cc.AddChannel(QosType.Unreliable);
            defaultTopology = new HostTopology(cc, 1);

            if (isServer)
                StartAsServer();

            if (isClient)
                StartAsClient();

            return true;
        }

        // listen for broadcasts
        public bool StartAsClient()
        {
            if (hostId != -1 || running)
            {
                if (LogFilter.logWarn) { Debug.LogWarning("NetworkDiscovery StartAsClient already started"); }
                return false;
            }

            if (msgInBuffer == null)
            {
                if (LogFilter.logError) { Debug.LogError("NetworkDiscovery StartAsClient, NetworkDiscovery is not initialized"); }
                return false;
            }

            hostId = NetworkTransport.AddHost(defaultTopology, broadcastPort);
            if (hostId == -1)
            {
                if (LogFilter.logError) { Debug.LogError("NetworkDiscovery StartAsClient - addHost failed"); }
                return false;
            }

            byte error;
            NetworkTransport.SetBroadcastCredentials(hostId, broadcastKey, broadcastVersion, broadcastSubVersion, out error);

            running = true;
            isClient = true;
            if (LogFilter.logDebug) { Debug.Log("StartAsClient Discovery listening"); }
            return true;
        }

        // perform actual broadcasts
        public bool StartAsServer()
        {
            if (hostId != -1 || running)
            {
                if (LogFilter.logWarn) { Debug.LogWarning("NetworkDiscovery StartAsServer already started"); }
                return false;
            }

            hostId = NetworkTransport.AddHost(defaultTopology, 0);
            if (hostId == -1)
            {
                if (LogFilter.logError) { Debug.LogError("NetworkDiscovery StartAsServer - addHost failed"); }
                return false;
            }

            byte err;
            if (!NetworkTransport.StartBroadcastDiscovery(hostId, broadcastPort, broadcastKey, broadcastVersion, broadcastSubVersion, msgOutBuffer, msgOutBuffer.Length, broadcastInterval, out err))
            {
                if (LogFilter.logError) { Debug.LogError("NetworkDiscovery StartBroadcast failed err: " + err); }
                return false;
            }

            running = true;
            isServer = true;
            if (LogFilter.logDebug) { Debug.Log("StartAsServer Discovery broadcasting"); }
            DontDestroyOnLoad(gameObject);
            return true;
        }

        public void StopBroadcast()
        {
            if (hostId == -1)
            {
                if (LogFilter.logError) { Debug.LogError("NetworkDiscovery StopBroadcast not initialized"); }
                return;
            }

            if (!running)
            {
                Debug.LogWarning("NetworkDiscovery StopBroadcast not started");
                return;
            }
            if (isServer)
            {
                NetworkTransport.StopBroadcastDiscovery();
            }

            NetworkTransport.RemoveHost(hostId);
            hostId = -1;
            running = false;
            isServer = false;
            isClient = false;
            msgInBuffer = null;
            broadcastsReceived = null;
            if (LogFilter.logDebug) { Debug.Log("Stopped Discovery broadcasting"); }
        }

        void Update()
        {
            UpdateBroadcastData();

            if (hostId == -1)
                return;

            if (isServer)
                return;

            NetworkEventType networkEvent;
            do
            {
                int connectionId;
                int channelId;
                int receivedSize;
                byte error;
                networkEvent = NetworkTransport.ReceiveFromHost(hostId, out connectionId, out channelId, msgInBuffer, k_MaxBroadcastMsgSize, out receivedSize, out error);

                if (networkEvent == NetworkEventType.BroadcastEvent)
                {
                    NetworkTransport.GetBroadcastConnectionMessage(hostId, msgInBuffer, k_MaxBroadcastMsgSize, out receivedSize, out error);

                    string senderAddr;
                    int senderPort;
                    NetworkTransport.GetBroadcastConnectionInfo(hostId, out senderAddr, out senderPort, out error);

                    var recv = new NetworkBroadcastResult();
                    recv.serverAddress = senderAddr;
                    recv.broadcastData = new byte[receivedSize];
                    Buffer.BlockCopy(msgInBuffer, 0, recv.broadcastData, 0, receivedSize);
                    broadcastsReceived[senderAddr] = recv;

                    OnReceivedBroadcast(senderAddr, BytesToString(msgInBuffer));
                }
            }
            while (networkEvent != NetworkEventType.Nothing);
        }

        void OnDestroy()
        {
            if (isServer && running && hostId != -1)
            {
                NetworkTransport.StopBroadcastDiscovery();
                NetworkTransport.RemoveHost(hostId);
            }

            if (isClient && running && hostId != -1)
            {
                NetworkTransport.RemoveHost(hostId);
            }
        }

        #endregion
    }
}
