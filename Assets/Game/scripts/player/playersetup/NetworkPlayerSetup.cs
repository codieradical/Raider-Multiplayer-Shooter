using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Raider.Game.Player;
using Raider.Game.Saves;
using Raider;
using Raider.Game.Cameras;
using Raider.Game.Networking;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerChatManager))]
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NetworkAnimator))]
    [RequireComponent(typeof(PlayerResourceReferences))]
    public class NetworkPlayerSetup : NetworkBehaviour
    {
        public static NetworkPlayerSetup localPlayer;
        public GameObject graphicsObject;
        private PlayerData playerData;

        // Use this for initialization

        void Start()
        {
            playerData = GetComponent<PlayerData>();

            //If the player is a client, or is playing alone, add the moving mechanics.
            if (isLocalPlayer)
            {
                PlayerData.localPlayerData = playerData;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                PlayerData lobbyPlayerData = NetworkLobbyPlayerSetup.localPlayer.GetComponent<PlayerData>();

                PlayerData.localPlayerData.character = lobbyPlayerData.character;
                PlayerData.localPlayerData.name = lobbyPlayerData.name;
                PlayerData.localPlayerData.slot = lobbyPlayerData.slot;
                PlayerData.localPlayerData.gotData = true;

                SetupLocalPlayer();

                CmdUpdatePlayerSlot(PlayerData.localPlayerData.slot);
            }
            else
            {
                if (localPlayer != null)
                    localPlayer.CmdRequestSlot();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!playerData.gotData)
                CmdRequestSlot();
        }

        void SetupLocalPlayer()
        {
            localPlayer = this;
            localPlayer.CmdRequestSlot(); //Now that authority is established, issue this command.
            gameObject.AddComponent<MovementController>();
            playerData.animationController = gameObject.AddComponent<PlayerAnimationController>();
            playerData.gamePlayerController = gameObject.AddComponent<GamePlayerController>();
            CameraModeController.singleton.playerGameObject = gameObject;
            //CameraModeController.singleton.SetCameraMode(Session.saveDataHandler.GetSettings().perspective);
            playerData.gamePlayerController.UpdatePerspective(Session.userSaveDataHandler.GetSettings().perspective);
        }

        #region slot sync

        [Command]
        void CmdUpdatePlayerSlot(int newSlot)
        {
            Debug.Log("Client sent slot " + newSlot.ToString() + " for " + name);
            if (!playerData.gotData)
                UpdateLocalSlot(newSlot);
            RpcUpdatePlayerSlot(newSlot);
        }

        [Command]
        void CmdRequestSlot()
        {
            Debug.Log("Client requests slot for " + name);
            if (playerData.gotData)
            {
                Debug.Log("Sending a client slot for" + name);
                TargetUpdatePlayerSlot(connectionToClient, playerData.slot);
            }
            else
                Debug.LogWarning("I don't have that slot!");
        }

        [TargetRpc]
        void TargetUpdatePlayerSlot(NetworkConnection target, int newSlot)
        {
            UpdateLocalSlot(newSlot);
            Debug.Log("Recieved rpc slot " + newSlot.ToString() + " for " + name);
        }

        [ClientRpc]
        void RpcUpdatePlayerSlot(int newSlot)
        {
            UpdateLocalSlot(newSlot);
            Debug.Log("Recieved rpc slot " + newSlot.ToString() + " for " + name);
        }

        void UpdateLocalSlot(int value)
        {
            //If the client already has the slot, don't need to update.
            if (playerData.gotData)
                return;

            if (isLocalPlayer)
                return;

            playerData.slot = value;

            PlayerData lobbyPlayerData = NetworkGameManager.instance.GetPlayerDataBySlot(playerData.slot);
            playerData.character = lobbyPlayerData.character;
            playerData.name = lobbyPlayerData.name;
            playerData.team = lobbyPlayerData.team;
            playerData.isLeader = lobbyPlayerData.isLeader;
            playerData.isHost = lobbyPlayerData.isHost;
            playerData.gotData = true;

            Debug.Log("recieved slot " + value.ToString() + " for " + name);

            if (playerData.appearenceController == null)
                playerData.appearenceController = GetComponentInChildren<PlayerAppearenceController>();

            playerData.appearenceController.ReplaceGraphicsModel(playerData);
        }

        #endregion
    }
}
