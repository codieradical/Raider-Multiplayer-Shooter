using Raider.Common.Types;
using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.Player;
using Raider.Game.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Raider.Game.Networking
{

    public class NetworkGameManager : NetworkLobbyManager
    {

        #region singleton setup

        //This class already inherits a singleton...
        public static NetworkGameManager instance;
        public LobbySetup lobbySetup;
		public GametypeController activeGametype;

        void Start()
        {
            //Although this functionality is built into the network lobby manager,
            //It only works on update.
            DontDestroyOnLoad(this);
            lobbySetup = gameObject.AddComponent<LobbySetup>();

            onClientDisconnect += UpdateLobbyNameplates;
        }

        void Awake()
        {
            if (instance != null)
                Debug.LogError("More than one NetworkManager are active! What are you doing!!");
            instance = this;
        }

        void OnDestroy()
        {
#if !UNITY_EDITOR
            Debug.LogError("Something just destroyed the NetworkManager!");
#endif
            instance = null;
        }

        #endregion

        //This Queue is used to store function calls which will be processed next frame.
        //I really need to refactor this.
        public Queue<Action> actionQueue = new Queue<Action>();
        public delegate void NetworkMessage();

        void Update()
        {
            while(actionQueue.Count > 0)
            {
                actionQueue.Dequeue()();
            }
        }

        public List<PlayerData> Players
        {
            get
            {
                GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
                List<PlayerData> players = new List<PlayerData>();
                foreach(GameObject player in playerObjects)
                {
                    if(Scenario.InLobby && player.GetComponent<NetworkLobbyPlayer>())
                        players.Add(player.GetComponent<PlayerData>());
                    else if (!Scenario.InLobby && player.GetComponent<NetworkLobbyPlayer>() == null)
                        players.Add(player.GetComponent<PlayerData>());
                }
                return players;
            }
        }


		////DEBUG
		//private void OnGUI()
		//{
		//	for (int i = 0; i < Players.Count; i++)
		//	{
		//		UnityEngine.GUI.Label(new Rect(0, 100, 50, 20), "Name");
		//		UnityEngine.GUI.Label(new Rect(0, 130, 50, 20), "Health");
		//		UnityEngine.GUI.Label(new Rect(0, 160, 50, 20), "Alive");
		//		UnityEngine.GUI.Label(new Rect(0, 190, 50, 20), "Score");
		//		try
		//		{
		//			UnityEngine.GUI.Label(new Rect(i * 100 + 50, 100, 100, 20), Players[i].syncData.username);
		//			UnityEngine.GUI.Label(new Rect(i * 100 + 50, 130, 100, 20), Players[i].networkPlayerController.health.ToString());
		//			UnityEngine.GUI.Label(new Rect(i * 100 + 50, 160, 100, 20), Players[i].networkPlayerController.IsAlive.ToString());

		//			foreach(GametypeController.ScoreboardPlayer player in GametypeController.singleton.scoreboard)
		//			{
		//				if (player.name == Players[i].syncData.username)
		//					UnityEngine.GUI.Label(new Rect(i * 100 + 50, 190, 100, 20), player.score.ToString());
		//			}
		//		}
		//		catch (Exception ex) { }
		//	}
		//}

			//Called when a client leaves.
		public override void OnLobbyServerDisconnect(NetworkConnection conn)
		{
			if (activeGametype != null)
				activeGametype.UpdateScoreboardActivePlayers();
		}

		public bool IsLeader
        {
            get
            {
                if (PlayerData.localPlayerData != null)
                    if (PlayerData.localPlayerData.PlayerSyncData.isLeader)
                        return true;
                    else
                        return false;
                else
                    return true;
            }
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            PlayerData gamePlayerData = gamePlayer.GetComponent<PlayerData>();
            PlayerData lobbyPlayerData = lobbyPlayer.GetComponent<PlayerData>();
            gamePlayerData.PlayerSyncData = lobbyPlayerData.PlayerSyncData;
            gamePlayer.name = lobbyPlayerData.name;
            return true;
        }

        //public override onlobby

        //If the host loads a map, and it's the mainmenu, we're ready for the next match.
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);

			//if the newly loaded scene is the menu, and there's a lobby player already setup, and that player is not the host, send the ready up flag.
			if (SceneManager.GetActiveScene().name == lobbyScene && NetworkLobbyPlayerSetup.localPlayer != null && CurrentNetworkState != NetworkState.Host && CurrentNetworkState != NetworkState.Server)
			{
				NetworkLobbyPlayerSetup.localPlayer.GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
				NetworkLobbyPlayerSetup.localPlayer.CmdRequestLobbySetupUpdate();
			}
        }

        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            base.OnLobbyServerSceneChanged(sceneName);

            if(sceneName != lobbyScene)
            {
				//Reset the scoreboard...
				GametypeController.InstanceGametypeByEnum(lobbySetup.syncData.Gametype);

				StartCoroutine(activeGametype.InitialSpawnPlayers());

				foreach(PlayerData player in Players)
				{
					activeGametype.AddPlayerToScoreboard(player.PlayerSyncData.id);
				}
			}
            else
            {
                foreach(PlayerData player in Players)
                {
                    //If players have changed their team in game, only the host updates the lobby object.
                    //So now that the lobby is loaded again, update the other players.
                    player.RpcChangeTeam(player.PlayerSyncData.team);
                }
            }
        }

        /// <summary>
        /// Get the slot number from the lobby or game player object.
        /// </summary>
        /// <returns>Returns the int slot.</returns>
        public int GetMyPlayerId()
        {
            return PlayerData.localPlayerData.PlayerSyncData.id;
        }

        public PlayerData GetPlayerDataById(int gamePlayerSlot)
        {
            foreach(PlayerData player in Players)
            {
                if (player.GetComponent<PlayerData>().PlayerSyncData.id == gamePlayerSlot)
                    return player;
            }
            return null;
        }

        #region Lobby Methods

        //Called on a server or host when the server starts.
        public NetworkMessage onStartServer;
        public override void OnStartServer()
        {
            lobbySetup.syncData.gameOptions = new GametypeController.GameOptions();

            base.OnStartServer();
            if (onStartServer != null)
                onStartServer();
        }

        //Called on a client when they connect.
        public NetworkMessage onClientConnect;
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            if (onClientConnect != null)
                onClientConnect();
        }

        //Called on a server or host when the server stops.
        public NetworkMessage onStopServer;
        public override void OnStopServer()
        {
            lobbySetup.syncData.gameOptions = new GametypeController.GameOptions();

            base.OnStopServer();
            if(onStopServer != null)
                onStopServer();
        }

        //Called on the client when they disconnect.
        public NetworkMessage onClientDisconnect;
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            lobbySetup.syncData.gameOptions = new GametypeController.GameOptions();

            base.OnClientDisconnect(conn);
            if(onClientDisconnect != null)
                onClientDisconnect();
        }

        //Used to call SendReadyToBeginMessage on PlayerData from other classes.
        public void ReadyToBegin()
        {
            if (CurrentNetworkState == NetworkState.Client || CurrentNetworkState == NetworkState.Host)
                NetworkLobbyPlayerSetup.localPlayer.GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            foreach (PlayerController playerController in conn.playerControllers)
            {
                PlayerData playerData = playerController.gameObject.GetComponent<PlayerData>();

                if (playerData != null)
                {
                    NetworkLobbyPlayerSetup.localPlayer.GetComponent<PlayerChatManager>().CmdSendNotificationMessage("left the game.", playerData.PlayerSyncData.id);
                    break;
                }
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnLobbyServerPlayersReady()
        {
            NetworkLobbyPlayerSetup.localPlayer.RpcUpdateScenarioGametype();
            base.OnLobbyServerPlayersReady();
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            Scenario.instance.NetworkLoadedScene();
            base.OnLobbyClientSceneChanged(conn);
        }

        //TODO
        //IMPLEMENT COUNTDOWN TIMER
        //float countTimer = 0;

        //public override void OnLobbyServerPlayersReady()
        //{
        //    countTimer = Time.time + 5;
        //}

        //void Update()
        //{
        //    if (countTimer == 0)
        //        return;

        //    if (Time.time > countTimer)
        //    {
        //        countTimer = 0;
        //        ServerChangeScene(playScene);
        //    }
        //    else
        //    {
        //        Debug.Log("Counting down " + (countTimer - Time.time));
        //    }
        //}

        public void UpdateLobbyNameplates()
        {
            //If the player is in a lobby, use the player lobby objects to create nameplates.
            if (CurrentNetworkState != NetworkState.Offline)
            {
                LobbyHandler.DestroyAllPlayers();

                if (lobbySetup != null && lobbySetup.syncData != null && lobbySetup.syncData.gameOptions != null && lobbySetup.syncData.gameOptions.teamsEnabled)
                {
                    foreach (Gametypes.GametypeHelper.Team team in Enum.GetValues(typeof(Gametypes.GametypeHelper.Team)))
                    {
                        foreach (PlayerData playerData in Players)
                        {
                            if (playerData.PlayerSyncData.GotData && playerData.PlayerSyncData.team == team)
                                LobbyHandler.AddPlayer(playerData.PlayerSyncData);
                        }
                    }
                    foreach (PlayerData playerData in Players)
                    {
                        if (!playerData.PlayerSyncData.GotData)
                            LobbyHandler.AddLoadingPlayer();
                    }
                }
                else
                {
                    foreach (PlayerData playerData in Players)
                    {
                        if (playerData.PlayerSyncData.GotData)
                            LobbyHandler.AddPlayer(playerData.PlayerSyncData);
                        else
                            LobbyHandler.AddLoadingPlayer();
                    }
                }
            }
            //Otherwise, use their local data.
            else
            {
                if (Session.ActiveCharacter != null)
                {
                    LobbyHandler.DestroyAllPlayers();
                    PlayerData.SyncData nameplateData = new PlayerData.SyncData()
                    {
                        username = Session.userSaveDataHandler.GetUsername(),
                        isLeader = true,
                        Character = Session.ActiveCharacter,
                        id = 1
                    };
                    LobbyHandler.AddPlayer(nameplateData);
                }
            }
        }

        #endregion

        #region Updating NetworkState

        public enum NetworkState
        {
            Offline,
            Client,
            Host,
            Server
            //Matchmaking?
        }

        //In order to make sure these delegates are called when required,
        //Lobby disconnect methods need to be overriden.
        public NetworkMessage onNetworkStateOffline;
        public NetworkMessage onNetworkStateHost;
        public NetworkMessage onNetworkStateClient;
        public NetworkMessage onNetworkStateServer;

        public NetworkState CurrentNetworkState
        {
            get
            {
                //Find what state the network is in, by checking if the server and or client are running.
                if (NetworkServer.active)
                    if (NetworkClient.active)
                        return NetworkState.Host;
                    else
                        return NetworkState.Server;
                else if (NetworkClient.active)
                    return NetworkState.Client;
                else
                    return NetworkState.Offline;
            }

            set
            {
                //Call methods to switch state by starting/stopping communications.
                if (value == NetworkState.Client)
                {
                    StartClient();
                    if(onNetworkStateClient != null)
                        onNetworkStateClient();
                }
                else if (value == NetworkState.Host)
                {
                    StartHost();
                    if (onNetworkStateHost != null)
                        onNetworkStateHost();
                }
                else if (value == NetworkState.Server)
                {
                    StartServer();
                    if (onNetworkStateHost != null)
                        onNetworkStateServer();
                }
                else if (value == NetworkState.Offline)
                {
                    StopCommunications();
                    if (onNetworkStateOffline != null)
                        onNetworkStateOffline();
                }

                //Sometimes this works now, sometimes it needs another frame.
                UpdateLobbyNameplates();
                actionQueue.Enqueue(UpdateLobbyNameplates);
            }
        }

        //This method doesn't call UpdateLobbyNameplates, so it shouldn't be called directly.
        private void StopCommunications()
        {
            //If the network is active, figure out what's running, and stop it.
            if (isNetworkActive)
            {
                if (CurrentNetworkState == NetworkState.Client)
                    StopClient();
                else if (CurrentNetworkState == NetworkState.Host)
                    StopHost();
                else if (CurrentNetworkState == NetworkState.Server)
                    StopServer();

                UpdateLobbyNameplates();
            }
        }

        #endregion
    }
}
