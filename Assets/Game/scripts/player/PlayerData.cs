using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using Raider.Game.Networking;
using Raider.Game.Saves.User;

namespace Raider.Game.Player
{
    public class PlayerData : MonoBehaviour
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

        //Store references to the animators.
        public GameObject playerModel;
        //Reference to the first person object for the first person camera to use.
        public static string firstPersonPlayerModelName = "FirstPerson"; //Since this object is being found, keeping the name here helps.
        public GameObject firstPersonPlayerModel;
        //Also assigned in editor.
        public Animator playerModelAnimator;
        public Animator weaponModelAnimator;

        //public Weapons.Weapon weapon;

        public PlayerAnimationController animationController;
        public PlayerAppearenceController appearenceController;
        public GamePlayerController gamePlayerController;

        public bool paused;
        //Network Game Only.
        public int slot = -1;
        //Review accessors on this character.
        public UserSaveDataStructure.Character character;

        public string SerializedCharacter
        {
            get { return Serialization.Serialize(character); }
            set { character = Serialization.Deserialize<UserSaveDataStructure.Character>(value); }
        }

        public bool gotData;
        public bool isLeader;
        public bool isHost;
        public LobbySetup.Teams team = LobbySetup.Teams.None;

        private void Awake()
        {
            if(playerModel != null) //PlayerData is also used in lobby, where the player model is not assigned.
                appearenceController = playerModel.GetComponent<PlayerAppearenceController>();
            gamePlayerController = GetComponent<GamePlayerController>(); //Singleplayer assignment.
            animationController = GetComponent<PlayerAnimationController>(); //Singleplayer assignment.
        }
    }
}