using Raider.Game.Gametypes;
using Raider.Game.GUI.Scoreboard;
using Raider.Game.Networking;
using Raider.Game.Saves.User;
using Raider.Game.Scene;
using Raider.Game.Weapons;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    public class PlayerData : NetworkBehaviour
    {
        public static PlayerData localPlayerData;

        public bool IsLocalPlayer
        {
            get
            {
                if (this == localPlayerData)
                    return true;
                else
                    return false;
            }
        }

        //A reference to the root graphics object, assigned in editor.
        public GameObject graphicsObject;
        public GameObject playerModel; // Assigned In Editor To Begin
        public GameObject firstPersonPlayerModel; //Assigned in editor to begin.
        public NetworkPlayerController networkPlayerController; //Assigned in editor.
        public Animator sharedParametersAnimator; //Assigned in editor.

        public AnimationParametersController animationController; //Assigned in editor or on creation.
        public PlayerAppearenceController appearenceController; //Assigned In Editor to Begin
        public LocalPlayerController localPlayerController;

		private Armory.WeaponType activeWeaponType = Armory.WeaponType.Primary;
		public Armory.WeaponType ActiveWeaponType
		{
			get { return activeWeaponType; }
			set
			{
				activeWeaponType = value;
				primaryWeaponController.activeWeapon = false;
				secondaryWeaponController.activeWeapon = false;
				tertiaryWeaponController.activeWeapon = false;

				if (value == Armory.WeaponType.Primary)
					primaryWeaponController.activeWeapon = true;
				else if (value == Armory.WeaponType.Secondary)
					secondaryWeaponController.activeWeapon = true;
				else if (value == Armory.WeaponType.Tertiary)
					tertiaryWeaponController.activeWeapon = true;
			}
		}

		public WeaponController primaryWeaponController;
		public WeaponController secondaryWeaponController;
		public WeaponController tertiaryWeaponController;

        public bool paused;

		[SyncVar(hook = "OnSyncDataSynced")] public SyncData syncData;

        [Serializable]
        public class SyncData
        {
            //Sync Fields
            [SyncVar] public int id = -1;
            [SyncVar] public string username;
            [SyncVar] public string character; //This should really be private set, but I'm pretty sure that'd break the syncvar.
            [SyncVar] public bool isLeader;
            [SyncVar] public bool isHost;
            [SyncVar] public Gametypes.GametypeHelper.Team team = Gametypes.GametypeHelper.Team.None;

            //Properties
            public bool GotData
            {
                get
                {
                    if (id == -1)
                        return false;
                    else
                        return true;
                }
            }
            public UserSaveDataStructure.Character Character
            {
                get { return Serialization.Deserialize<UserSaveDataStructure.Character>(character); }
                set { character = Serialization.Serialize(value); }
            }

        }

        void OnSyncDataSynced(SyncData value)
        {
            syncData = value;
            Debug.Log("Synced data for id " + syncData.id.ToString() + ", player " + syncData.username);
            gameObject.name = syncData.username;

            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

        private void Awake()
        {
            if(playerModel != null) //PlayerData is also used in lobby, where the player model is not assigned.
                appearenceController = playerModel.GetComponent<PlayerAppearenceController>();
            localPlayerController = GetComponent<LocalPlayerController>(); //Singleplayer assignment.
            animationController = GetComponent<AnimationParametersController>(); //Singleplayer assignment.
        }

        [Command]
        public void CmdChangeTeam(GametypeHelper.Team newTeam)
        {
            if (newTeam == GametypeHelper.Team.None)
                return;
            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
                return;
            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.clientTeamChangingGame && !Scenario.InLobby)
                return;
            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.clientTeamChangingGame && Scenario.InLobby)
                return;

			GametypeHelper.Team[] availableTeams = (GametypeHelper.Team[])Enum.GetValues(typeof(GametypeHelper.Team));
            Array.Resize(ref availableTeams, NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.maxTeams - 1);

            foreach(GametypeHelper.Team team in availableTeams)
            {
                if(team == newTeam)
                {
                    syncData.team = team;
                    NetworkGameManager.instance.UpdateLobbyNameplates();
                    RpcChangeTeam(team);

                    if (appearenceController != null)
                        appearenceController.UpdatePlayerAppearence(syncData);

                    //If the player is not in lobby, have the server update their lobby player for later.
                    if(!Scenario.InLobby)
                    {
						//Remove the old team item, add the new.
						GametypeController.singleton.AddPlayerToScoreboard(syncData.id);
						GametypeController.singleton.UpdateScoreboardActivePlayers();

                        foreach(NetworkLobbyPlayerSetup lobbyPlayer in FindObjectsOfType<NetworkLobbyPlayerSetup>())
                        {
                            if(lobbyPlayer.playerData.syncData.id == syncData.id)
                            {
                                lobbyPlayer.playerData.RpcChangeTeam(team);
                                break;
                            }
                        }
                    }
                }
            }
        }

        [ClientRpc]
        public void RpcChangeTeam(Gametypes.GametypeHelper.Team team)
        {
            syncData.team = team;
            NetworkGameManager.instance.UpdateLobbyNameplates();

            if (appearenceController != null)
                appearenceController.UpdatePlayerAppearence(syncData);
        }
    }
}