using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using Raider.Game.Networking;
using Raider.Game.Saves.User;
using UnityEngine.Networking;
using Raider.Game.Gametypes;
using Raider.Game.Scene;
using System;
using Raider.Game.GUI.Components;

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
        public Animator sharedParametersAnimator; //Assigned in editor.

        //public Weapons.Weapon weapon;

        public AnimationParametersController animationController; //Assigned in editor or on creation.
        public PlayerAppearenceController appearenceController; //Assigned In Editor to Begin
        public LocalPlayerController gamePlayerController;

        public bool paused;

        [SyncVar(hook = "OnSyncDataSynced")] public SyncData syncData;

        [System.Serializable]
        public class SyncData
        {
            //Sync Fields
            [SyncVar] public int id = -1;
            [SyncVar] public string username;
            [SyncVar] public string character; //This should really be private set, but I'm pretty sure that'd break the syncvar.
            [SyncVar] public bool isLeader;
            [SyncVar] public bool isHost;
            [SyncVar] public Gametype.Teams team = Gametype.Teams.None;

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
            gamePlayerController = GetComponent<LocalPlayerController>(); //Singleplayer assignment.
            animationController = GetComponent<AnimationParametersController>(); //Singleplayer assignment.
        }

        [Command]
        public void CmdChangeTeam(Gametype.Teams newTeam)
        {
            if (newTeam == Gametype.Teams.None)
                return;
            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
                return;
            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.clientTeamChangingGame && !Scenario.InLobby)
                return;
            if (!NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.clientTeamChangingGame && Scenario.InLobby)
                return;

            Gametype.Teams[] availableTeams = (Gametype.Teams[])Enum.GetValues(typeof(Gametype.Teams));
            Array.Resize(ref availableTeams, NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamOptions.maxTeams - 1);

            foreach(Gametype.Teams team in availableTeams)
            {
                if(team == newTeam)
                {
                    syncData.team = team;
                    NetworkGameManager.instance.UpdateLobbyNameplates();
                    RpcChangeTeam(team);

                    if (appearenceController != null)
                        appearenceController.UpdatePlayerAppearence(syncData);
                }
            }

        }

        [ClientRpc]
        public void RpcChangeTeam(Gametype.Teams team)
        {
            syncData.team = team;
            NetworkGameManager.instance.UpdateLobbyNameplates();

            if (appearenceController != null)
                appearenceController.UpdatePlayerAppearence(syncData);
        }
    }
}