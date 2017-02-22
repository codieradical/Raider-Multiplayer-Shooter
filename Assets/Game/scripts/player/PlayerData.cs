using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using Raider.Game.Networking;
using Raider.Game.Saves.User;
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
        //Reference to the first person object for the first person camera to use.
        public static string firstPersonPlayerModelName = "FirstPerson"; //Since this object is being found, keeping the name here helps.
        public GameObject firstPersonPlayerModel;
        public Animator playerModelAnimator;
        public Animator weaponModelAnimator;

        //public Weapons.Weapon weapon;

        public PlayerAnimationController animationController; //Assigned In Editor
        public PlayerAppearenceController appearenceController; //Assigned In Editor to Begin
        public GamePlayerController gamePlayerController;

        public bool paused;

        [SyncVar(hook = "OnSyncDataSync")] public SyncData syncData;

        [System.Serializable]
        public class SyncData
        {
            //Sync Fields
            [SyncVar] public int id = -1;
            [SyncVar] public string username;
            [SyncVar] public string character; //This should really be private set, but I'm pretty sure that'd break the syncvar.
            [SyncVar] public bool isLeader;
            [SyncVar] public bool isHost;
            //public LobbySetup.Teams team = LobbySetup.Teams.None;

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

        void OnSyncDataSync(SyncData value)
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
            gamePlayerController = GetComponent<GamePlayerController>(); //Singleplayer assignment.
            animationController = GetComponent<PlayerAnimationController>(); //Singleplayer assignment.
        }
    }
}