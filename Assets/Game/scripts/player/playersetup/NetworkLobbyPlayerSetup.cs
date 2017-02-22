using UnityEngine.Networking;
using UnityEngine;
using Raider.Game.Scene;
using Raider.Game.Networking;
using Raider.Game.Saves.User;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(PlayerChatManager))]
    [System.Serializable]
    public class NetworkLobbyPlayerSetup : NetworkBehaviour
    {
        public static NetworkLobbyPlayerSetup localPlayer;
        public PlayerData playerData;

        public delegate void OnLobbyPlayer();
        public static OnLobbyPlayer onLocalLobbyPlayerStart;

        public override void OnStartServer()
        {
            playerData = GetComponent<PlayerData>();
        }
        public override void OnStartClient()
        {
            playerData = GetComponent<PlayerData>();
        }

        public override void OnStartLocalPlayer()
        {
            localPlayer = this;
            PlayerData.localPlayerData = playerData;

            bool _isHost = false; //Update Local Data handles full assignment.
            if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host || NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Server)
                _isHost = true;

            //If the player is hosting (if networkserver is active), isLeader will be true.
            UpdateLocalData(GetComponent<NetworkLobbyPlayer>().slot, Session.userSaveDataHandler.GetUsername(), Session.ActiveCharacter, NetworkServer.active, _isHost);

            if(!_isHost)
                CmdRequestLobbySetupUpdate(); //Retrieve the current lobby setup from the server.

            //If the player is not the host, they're automatically set to ready.
            //This means the host's ready flag starts the game.
            if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
                GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();

            if (onLocalLobbyPlayerStart != null)
                onLocalLobbyPlayerStart();
        }

        //If a player is remove from the scene, update the lobby!
        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            NetworkGameManager.instance.actionQueue.Enqueue(NetworkGameManager.instance.UpdateLobbyNameplates);
        }

        void UpdateLocalData(int _id, string _username, UserSaveDataStructure.Character _character, bool _isLeader, bool _isHost)
        {
            Debug.Log("Updated Local Data. Slot: " + _id + "username: " + _username);
            playerData.syncData.id = _id;
            playerData.syncData.username = _username;
            playerData.syncData.Character = _character;
            playerData.syncData.isLeader = _isLeader;
            playerData.syncData.isHost = _isHost;
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        public void Update()
        {
            if (!playerData.syncData.GotData && !isLocalPlayer)
            {
                if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host || NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Server)
                {
                    TargetRequestSyncData(connectionToClient);
                }
                else if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client) // This elif isn't really necessary.
                {
                    localPlayer.CmdRequestAllSyncData(); //Called on the local player so the server knows who to target.
                }
            }
        }

        [TargetRpc]
        public void TargetSendClientSyncData(NetworkConnection target, PlayerData.SyncData _syncData)
        {
            //If for some reason the local player is sent to itself (sometimes unavoidable), ignore it.
            if (playerData.isLocalPlayer)
                return;

            playerData.syncData = _syncData;
            Debug.Log("Recieved sync data on for player " + playerData.syncData.username);
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        [TargetRpc]
        public void TargetRequestSyncData(NetworkConnection target)
        {
            if (playerData.syncData.GotData)
                CmdRecieveSyncData(playerData.syncData);
        }

        [Command]
        public void CmdRecieveSyncData(PlayerData.SyncData _syncData)
        {
            playerData.syncData = _syncData;
            gameObject.name = playerData.syncData.username;
            NetworkGameManager.instance.UpdateLobbyNameplates();
            GetComponent<PlayerChatManager>().CmdSendNotificationMessage("joined the game.", playerData.syncData.id);

            //Now that the client has sent the server it's data, the server can send back other clients' data.
            foreach (PlayerData player in NetworkGameManager.instance.Players)
            {
                if (player.syncData.GotData && player != playerData)
                    player.GetComponent<NetworkLobbyPlayerSetup>().TargetSendClientSyncData(connectionToClient, player.syncData);
            }
        }

        [Command] //As a backup, if for some reason the player still lacks sync data, they can request an update on all.
        public void CmdRequestAllSyncData()
        {
            Debug.LogWarning("A player (" + playerData.syncData.username + ")just requested all sync data.");
            foreach(PlayerData player in NetworkGameManager.instance.Players)
            {
                if (player.syncData.GotData && player.connectionToClient != connectionToClient) //If the server has data on the player and it's not the target player.
                {
                    TargetSendClientSyncData(connectionToClient, player.syncData); //Send the sync data.
                    Debug.LogWarning("Sending " + playerData.syncData.username + " sync data for " + player.syncData.username);
                }
            }
        }

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
            NetworkGameManager.instance.lobbySetup.RecieveLobbySetupUpdate(gametype, selectedScene, network);
        }

        [Command]
        public void CmdSendLobbySetup(string gametype, string network, string selectedScene)
        {
            //Allow the host to switch scene on the network manager.
            NetworkGameManager.instance.playScene = selectedScene;
            RpcSendLobbySetup(gametype, network, selectedScene);
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