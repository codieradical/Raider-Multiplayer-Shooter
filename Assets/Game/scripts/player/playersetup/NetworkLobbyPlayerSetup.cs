using Raider.Game.Saves;
using UnityEngine.Networking;
using UnityEngine;
using Raider.Game.Scene;
using Raider.Game.Networking;
using System;

namespace Raider.Game.Player
{
    [System.Serializable]
    public class NetworkLobbyPlayerSetup : NetworkBehaviour
    {
        public static NetworkLobbyPlayerSetup localPlayer;
        public PlayerData playerData;

        public delegate void OnLobbyPlayer();
        public static OnLobbyPlayer onLocalLobbyPlayerStart;

        void Start()
        {
            transform.SetParent(NetworkGameManager.instance.lobbyGameObject.transform);
            playerData = GetComponent<PlayerData>();

            if (isLocalPlayer)
            {
                gameObject.AddComponent<PlayerChatManager>();
                localPlayer = this;

                bool _isHost = false; //Update Local Data handles full assignment.
                if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host || NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Server)
                    _isHost = true;

                //If the player is hosting (if networkserver is active), isLeader will be true.
                UpdateLocalData(GetComponent<NetworkLobbyPlayer>().slot, Session.saveDataHandler.GetUsername(), Session.activeCharacter, NetworkServer.active, _isHost);

                //I don't think this is necessary, I can probably just call the command.
                //if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host)
                //    RpcRecieveUpdateFromServer(playerData.name, playerData.SerializedCharacter, playerData.isLeader, playerData.isHost);
                //else
                    CmdUpdateServer(playerData.slot, playerData.name, playerData.SerializedCharacter, playerData.isLeader, playerData.isHost);

                //If the player is not the host, they're automatically set to ready.
                //This means the host's ready flag starts the game.
                if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
                {
                    GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
                    Debug.Log("Requesting Lobby Update.");
                    CmdRequestLobbySetupUpdate();
                }

                if (onLocalLobbyPlayerStart != null)
                    onLocalLobbyPlayerStart();
            }
        }

        //If a player is remove from the scene, update the lobby!
        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            NetworkGameManager.instance.actionQueue.Enqueue(NetworkGameManager.instance.UpdateLobbyNameplates);
        }

        #region player data syncing

        void UpdateLocalData(int _slot, string _username, SaveDataStructure.Character _character, bool _isLeader, bool _isHost)
        {
            playerData.slot = _slot;
            playerData.name = _username;
            playerData.character = _character;
            playerData.isLeader = _isLeader;
            playerData.isHost = _isHost;
            playerData.gotData = true;
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        [Command]
        void CmdUpdateServer(int _slot, string _username, string _serializedCharacter, bool _isLeader, bool _isHost)
        {
            RpcRecieveUpdateFromServer(_slot, _username, _serializedCharacter, _isLeader, _isHost);
            UpdateLocalData(_slot, _username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter), _isLeader, _isHost);
            GetComponent<PlayerChatManager>().CmdSendNotificationMessage("joined the game.", playerData.slot);

            //If the client has sent their player data to the server, 
            //that means it's spawned and ready to recieve data regarding other players.
            //So now the server sends that data using the Client RPC.
            UpdateClientPlayerDataObjects();
        }

        [ClientRpc]
        public void RpcRecieveUpdateFromServer(int _slot, string _username, string _serializedCharacter, bool _isLeader, bool _isHost)
        {
            UpdateLocalData(_slot, _username, Serialization.Deserialize<SaveDataStructure.Character>(_serializedCharacter), _isLeader, _isHost);
        }

        [Server]
        public static void UpdateClientPlayerDataObjects()
        {
            foreach (NetworkLobbyPlayerSetup player in NetworkGameManager.instance.LobbyPlayers)
            {
                PlayerData playerData = player.GetComponent<PlayerData>();
                if (playerData.gotData)
                {
                    player.RpcRecieveUpdateFromServer(playerData.slot, playerData.name, playerData.SerializedCharacter, playerData.isLeader, playerData.isHost);
                }
            }
        }
        #endregion

        #region LobbySetup Syncing (Refactor Me!)

        [Command]
        void CmdRequestLobbySetupUpdate()
        {
            Debug.Log("Sending Lobby Data.");
            TargetSendLobbySetup(connectionToClient, NetworkGameManager.instance.lobbySetup.GametypeString, NetworkGameManager.instance.lobbySetup.Network, NetworkGameManager.instance.lobbySetup.SelectedScene);
        }

        //If a new player joins the lobby, this is used to send them the details.
        //I can't get this to work so instead I'm just updating all clients.
        //This should be refactored.
        [TargetRpc]
        public void TargetSendLobbySetup(NetworkConnection conn, string gametype, string network, string selectedScene)
        {
            Debug.Log("Recieved lobby data");
            NetworkGameManager.instance.lobbySetup.GametypeString = gametype;
            NetworkGameManager.instance.lobbySetup.Network = network;
            NetworkGameManager.instance.lobbySetup.SelectedScene = selectedScene;
        }

        //If the host changes the lobby setup, this sends the new details to the clients.
        [ClientRpc]
        public void RpcSendLobbySetup(string gametype, string network, string selectedScene)
        {
            //Hosts have a client and a server, but they don't need updating.
            //NetworkState.Client represents clients only.
            if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
            {
                Debug.Log("Recieved lobby data");
                NetworkGameManager.instance.lobbySetup.GametypeString = gametype;
                NetworkGameManager.instance.lobbySetup.Network = network;
                NetworkGameManager.instance.lobbySetup.SelectedScene = selectedScene;
            }
        }

        [ClientRpc]
        public void RpcUpdateScenarioGametype()
        {
            Scenario.instance.currentGametype = NetworkGameManager.instance.lobbySetup.Gametype;
        }

        #endregion
    }
}