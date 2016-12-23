using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using Raider.Game.Networking;

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

        public PlayerAppearenceController appearenceController;
        public PlayerManager playerManager;

        public bool paused;
        //Network Game Only.
        public int slot = -1;
        //Review accessors on this character.
        public SaveDataStructure.Character character;

        public string SerializedCharacter
        {
            get { return Serialization.Serialize(character); }
            set { character = Serialization.Deserialize<SaveDataStructure.Character>(value); }
        }

        public bool gotData;
        public bool isLeader;
        public bool isHost;
        public LobbySetup.Teams team = LobbySetup.Teams.None;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            appearenceController = GetComponent<PlayerAppearenceController>();
            playerManager = GetComponent<PlayerManager>(); //Singleplayer assignment.
        }
    }
}