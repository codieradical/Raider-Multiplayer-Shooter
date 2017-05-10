using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.Networking;
using Raider.Game.Saves.User;
using Raider.Game.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(PlayerChatManager))]
    [Serializable]
    public class NetworkLobbyPlayerSetup : NetworkBehaviour
    {
        public static NetworkLobbyPlayerSetup localPlayer;
        public PlayerData playerData;
        [SyncVar] public bool serverGotPlayerData = false;

        public delegate void OnLobbyPlayer();
        public static OnLobbyPlayer onLocalLobbyPlayerStart;

        public override void OnStartServer()
        {
            playerData = GetComponent<PlayerData>();

			//Dont think this is required.
            if (isLocalPlayer)
                serverGotPlayerData = true;
        }
        public override void OnStartClient()
        {
            playerData = GetComponent<PlayerData>();
        }

		public void OnEnable()
		{
			if (isLocalPlayer && !playerData.syncData.isLeader)
				GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
                
		    if(isLocalPlayer)
				CmdRequestLobbySetupUpdate();
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
            playerData.PlayerSyncData.id = _id;
            playerData.PlayerSyncData.username = _username;
            playerData.PlayerSyncData.Character = _character;
            playerData.PlayerSyncData.isLeader = _isLeader;
            playerData.PlayerSyncData.isHost = _isHost;
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        public void Update()
        {
            if (playerData != null && playerData.PlayerSyncData != null && !playerData.PlayerSyncData.GotData && !isLocalPlayer)
            {
                if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host || NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Server)
                {
                    TargetRequestSyncData(connectionToClient);
                }
                else if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client) // This elif isn't really necessary.
                {
                    if(serverGotPlayerData) //Be careful, when this method and the above method are called around the same time, the responses can loop.
                        localPlayer.CmdRequestAllSyncData(); //Called on the local player so the server knows who to target.
                }
            }
        }

        [TargetRpc]
        public void TargetSendClientSyncData(NetworkConnection target, PlayerData.SyncData _syncData)
        {
            //If for some reason the local player is sent to itself (sometimes unavoidable), ignore it.
            if (playerData.isLocalPlayer || playerData.PlayerSyncData.GotData)
                return;

            playerData.PlayerSyncData = _syncData;
            Debug.Log("Recieved sync data on for player " + playerData.PlayerSyncData.username);
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        [TargetRpc]
        public void TargetRequestSyncData(NetworkConnection target)
        {
            if (playerData.PlayerSyncData.GotData)
                CmdRecieveSyncData(playerData.PlayerSyncData);
        }

        [Command]
        public void CmdRecieveSyncData(PlayerData.SyncData _syncData)
        {
            if (playerData.PlayerSyncData.GotData) //For some reason this can be called more than once.
                return;

            playerData.PlayerSyncData = _syncData;
            gameObject.name = playerData.PlayerSyncData.username;
            serverGotPlayerData = true;
            NetworkGameManager.instance.UpdateLobbyNameplates();
            GetComponent<PlayerChatManager>().ServerSendNotificationMessage("joined the game.", playerData.PlayerSyncData.id);

            //Now that the client has sent the server it's data, the server can send back other clients' data.
            foreach (PlayerData player in NetworkGameManager.instance.Players)
            {
                if (player.PlayerSyncData.GotData && player != playerData)
                    player.GetComponent<NetworkLobbyPlayerSetup>().TargetSendClientSyncData(connectionToClient, player.PlayerSyncData);
            }

            //If teams are enabled, assign the player a team.
            if(NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
            {
				GametypeHelper.Team smallestTeam = GametypeHelper.Team.Red;
				List<GametypeHelper.Team> activeTeams = new List<GametypeHelper.Team>();

                int smallestTeamSize = 0;
                int currentTeamSize;
                foreach(GametypeHelper.Team team in Enum.GetValues(typeof(GametypeHelper.Team)))
                {
                    currentTeamSize = 0;
                    foreach (PlayerData player in NetworkGameManager.instance.Players)
                    {
                        if (player.PlayerSyncData.team == team)
                            currentTeamSize++;
                    }

					if (currentTeamSize > 0)
						activeTeams.Add(team);

                    if(currentTeamSize < smallestTeamSize && currentTeamSize > 0)
                    {
                        smallestTeamSize = currentTeamSize;
                        smallestTeam = team;
                    }
                }

				if(activeTeams.Count < 2)
				{
					if (activeTeams[0] == GametypeHelper.Team.Red)
						playerData.PlayerSyncData.team = GametypeHelper.Team.Blue;
					else
						playerData.PlayerSyncData.team = GametypeHelper.Team.Red;
				}
				else
					playerData.PlayerSyncData.team = smallestTeam;

                playerData.RpcChangeTeam(playerData.PlayerSyncData.team);
            }
        }

        [Command] //As a backup, if for some reason the player still lacks sync data, they can request an update on all.
        public void CmdRequestAllSyncData()
        {
            Debug.LogWarning("A player (" + playerData.PlayerSyncData.username + ") just requested all sync data.");
            foreach(PlayerData player in NetworkGameManager.instance.Players)
            {
                if (player.PlayerSyncData.GotData && player.connectionToClient != connectionToClient) //If the server has data on the player and it's not the target player.
                {
                    TargetSendClientSyncData(connectionToClient, player.PlayerSyncData); //Send the sync data.
                    Debug.LogWarning("Sending " + playerData.PlayerSyncData.username + " sync data for " + player.PlayerSyncData.username);
                }
            }
        }

        #region LobbySetup Syncing (Refactor Me!)

        [Command]
        public void CmdRequestLobbySetupUpdate()
        {
            Debug.Log("Sending Lobby Data.");
            TargetSendLobbySetup(connectionToClient, NetworkGameManager.instance.lobbySetup.syncData);
        }

        //If a new player joins the lobby, this is used to send them the details.
        //I can't get this to work so instead I'm just updating all clients.
        //This should be refactored.
        [TargetRpc]
        public void TargetSendLobbySetup(NetworkConnection conn, LobbySetup.SyncData syncData)
        {
            Debug.Log("Recieved lobby data");
            NetworkGameManager.instance.lobbySetup.RecieveLobbySetupUpdate(syncData);
            LobbySetupPane.instance.UpdatePaneData();
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        [Command]
        public void CmdSendLobbySetup(LobbySetup.SyncData syncData)
        {
            if (!playerData.PlayerSyncData.isLeader)
                return;

            //Update teams.
            if(syncData.gameOptions.teamsEnabled)
            {
                for(int i = NetworkGameManager.instance.Players.Count - 1; i >= 0; i--)
                {
                    if (NetworkGameManager.instance.Players[i].PlayerSyncData.team == GametypeHelper.Team.None)
                    {
                        if ((i + 2) % 2 == 1)
                        {
                            NetworkGameManager.instance.Players[i].PlayerSyncData.team = GametypeHelper.Team.Red;
                            NetworkGameManager.instance.Players[i].RpcChangeTeam(GametypeHelper.Team.Red);
                        }
                        else
                        {
                            NetworkGameManager.instance.Players[i].PlayerSyncData.team = GametypeHelper.Team.Blue;
                            NetworkGameManager.instance.Players[i].RpcChangeTeam(GametypeHelper.Team.Blue);
                        }
                    }
                }
            }
            else
            {
                foreach(PlayerData player in NetworkGameManager.instance.Players)
                {
                    if (player.PlayerSyncData.team != GametypeHelper.Team.None)
                    {
                        player.PlayerSyncData.team = GametypeHelper.Team.None;
                        player.RpcChangeTeam(GametypeHelper.Team.None);
                    }
                }
            }

            //Allow the host to switch scene on the network manager.
            NetworkGameManager.instance.playScene = syncData.SelectedScene;
            NetworkGameManager.instance.lobbySetup.syncData = syncData;
            RpcSendLobbySetup(syncData);

            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        //If the host changes the lobby setup, this sends the new details to the clients.
        [ClientRpc]
        public void RpcSendLobbySetup(LobbySetup.SyncData syncData)
        {
            //Hosts have a client and a server, but they don't need updating.
            //NetworkState.Client represents clients only.
            if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
            {
                Debug.Log("Recieved lobby data");
                NetworkGameManager.instance.lobbySetup.syncData = syncData;
                LobbySetupPane.instance.UpdatePaneData();
                NetworkGameManager.instance.UpdateLobbyNameplates();
            }
        }

        [ClientRpc]
        public void RpcUpdateScenarioGametype()
        {
            Scenario.instance.currentGametype = NetworkGameManager.instance.lobbySetup.syncData.Gametype;
        }

        #endregion
    }
}