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

        public GameObject graphicsObject;

        public Animator animator;

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
            animator = GetComponent<Animator>();
            appearenceController = GetComponent<PlayerAppearenceController>();
            gamePlayerController = GetComponent<GamePlayerController>(); //Singleplayer assignment.
            animationController = GetComponent<PlayerAnimationController>(); //Singleplayer assignment.
        }
    }
}