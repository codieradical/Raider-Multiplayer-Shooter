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

        //Called when a client leaves.
        public NetworkMessage onLobbyServerDisconnect;
		public override void OnLobbyServerDisconnect(NetworkConnection conn)
		{
            if (onLobbyServerDisconnect != null)
                onLobbyServerDisconnect();
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
					activeGametype.AddOrReactivateScoreboardPlayer(player.PlayerSyncData.id,player.PlayerSyncData.team);
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

		#region spawning

		public static List<Tuple<GametypeHelper.Team, List<Vector3>>> spawnPoints = new List<Tuple<GametypeHelper.Team, List<Vector3>>>();

		public static void RegisterSpawnPoint(Vector3 point, GametypeHelper.Team team)
		{
			for (int i = 0; i < spawnPoints.Count; i++)
			{
				if (spawnPoints[i].First == team)
				{
					spawnPoints[i].Second.Add(point);
					return;
				}
			}

			spawnPoints.Add(new Tuple<GametypeHelper.Team, List<Vector3>>(team, new List<Vector3>() { point }));
		}

		public static void UnregisterSpawnPoint(Vector3 point, GametypeHelper.Team team)
		{
			for (int i = 0; i < spawnPoints.Count; i++)
			{
				if (spawnPoints[i].First == team)
				{
					spawnPoints[i].Second.Remove(point);
					return;
				}
			}
		}

		public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
		{
			if (gamePlayerPrefab == null)
			{
				if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
				return null;
			}

			if (gamePlayerPrefab.GetComponent<NetworkIdentity>() == null)
			{
				if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
				return null;
			}

			GameObject player;

			GametypeHelper.Team team = GametypeHelper.Team.None;
			foreach(PlayerData playerData in Players) //This loop isn't working. Fix me!
			{
				if (playerData.connectionToClient == conn)
					team = playerData.syncData.team;
			}

			Vector3 startPos = GetSpawnPoint(team);
			if (startPos != null)
			{
				player = Instantiate(gamePlayerPrefab, startPos, Quaternion.identity);
			}
			else
			{
				player = Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
			}

			return player;
		}

		public static Vector3 GetSpawnPoint(GametypeHelper.Team team)
		{
			for (int i = 0; i < spawnPoints.Count; i++)
			{
				if (spawnPoints[i].First == team)
				{
					if (spawnPoints[i].Second.Count > 0)
					{
						int element = UnityEngine.Random.Range(0, spawnPoints[i].Second.Count - 1);
						return spawnPoints[i].Second[element];
					}
					else break;
				}
			}

			for (int i = 0; i < spawnPoints.Count; i++)
			{
				if (spawnPoints[i].First == GametypeHelper.Team.None)
				{
					if (spawnPoints[i].Second.Count > 0)
					{
						int element = UnityEngine.Random.Range(0, spawnPoints[i].Second.Count - 1);
						return spawnPoints[i].Second[element];
					}
					else break;
				}
			}

			return Vector3.zero;
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
