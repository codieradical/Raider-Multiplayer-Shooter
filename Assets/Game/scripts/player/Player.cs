using UnityEngine;
using Raider.Game.Networking;
using UnityEngine.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;
using System.Collections;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(Animator))]
    public class Player : NetworkBehaviour
    {
        [System.Serializable]
        private class RaceGraphics
        {
            public GameObject xRaceGraphics;
            public GameObject yRaceGraphics;
            public Avatar xRaceAvatar;
            public Avatar yRaceAvatar;

            public void CheckAllGraphicsPresent()
            {
                if (xRaceGraphics == null || yRaceGraphics == null || xRaceAvatar == null || yRaceAvatar == null)
                    Debug.LogError("The player is missing a model prefab or avatar!!!");
            }

            public GameObject GetGraphicsByRace(SaveDataStructure.Character.Race race)
            {
                if (race == SaveDataStructure.Character.Race.X)
                    return xRaceGraphics;
                else if (race == SaveDataStructure.Character.Race.Y)
                    return yRaceGraphics;
                else
                {
                    Debug.LogError("Couldn't find graphics for race " + race.ToString());
                    return null;
                }
            }

            public Avatar GetAvatarByRace(SaveDataStructure.Character.Race race)
            {
                if (race == SaveDataStructure.Character.Race.X)
                    return xRaceAvatar;
                else if (race == SaveDataStructure.Character.Race.Y)
                    return yRaceAvatar;
                else
                {
                    Debug.LogError("Couldn't find avatar for race " + race.ToString());
                    return null;
                }
            }
        }

        [SerializeField]
        private RaceGraphics raceGraphics;
        private Animator animatorInstance;
        public bool lockCursor = true;
        bool paused;
        public int slot = -1;
        public SaveDataStructure.Character character;

        public static Player localPlayer;
        public GameObject graphicsObject;

        // Use this for initialization
        void Start()
        {
            animatorInstance = GetComponent<Animator>();
            raceGraphics.CheckAllGraphicsPresent();

            //If the player is a client, or is playing alone, add the moving mechanics.
            if (isLocalPlayer || Networking.NetworkManager.instance.CurrentNetworkState == Networking.NetworkManager.NetworkState.Offline)
            {
                if (lockCursor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                SetupLocalPlayer();

                character = LobbyPlayerData.localPlayer.character;
                name = LobbyPlayerData.localPlayer.name;
                slot = LobbyPlayerData.localPlayer.GetComponent<NetworkLobbyPlayer>().slot;
                gotSlot = true;

                CmdUpdatePlayerSlot(slot);

                UpdatePerspective(character.chosenPlayerPerspective);
            }
            else
            {
                if(localPlayer != null)
                    localPlayer.CmdRequestSlots();
            }
        }

        public void UpdatePerspective(CameraModeController.CameraModes newPerspective)
        {
            character.chosenPlayerPerspective = newPerspective;
            CameraModeController.singleton.SetCameraMode(newPerspective);

            StartCoroutine(PauseNewCameraController());

            if (newPerspective == CameraModeController.CameraModes.FirstPerson && graphicsObject != null)
            {
                Destroy(graphicsObject);
                animatorInstance.avatar = null;
            }
            else if (newPerspective != CameraModeController.CameraModes.FirstPerson && graphicsObject == null)
                SetupGraphicsModel();
        }

        IEnumerator PauseNewCameraController()
        {
            yield return 0;
            yield return 0;
            if(paused)
                CameraModeController.singleton.GetCameraController().enabled = false;
        }

        void SetupGraphicsModel()
        {
            animatorInstance.enabled = false;

            //Spawn the graphics.
            graphicsObject = Instantiate(raceGraphics.GetGraphicsByRace(character.race)) as GameObject;
            graphicsObject.transform.SetParent(this.transform, false);

            //Setup the animator
            animatorInstance.avatar = raceGraphics.GetAvatarByRace(character.race);

            //Update the colors, emblem.
            graphicsObject.GetComponent<PlayerAppearenceController>().UpdatePlayerAppearence(transform.name, character);

            animatorInstance.enabled = true;
        }

        void SetupLocalPlayer()
        {
            localPlayer = this;
            localPlayer.CmdRequestSlots(); //Now that authority is established, issue this command.
            gameObject.AddComponent<MovementController>();
            gameObject.AddComponent<PlayerAnimationController>();
            CameraModeController.singleton.playerGameObject = gameObject;
            CameraModeController.singleton.SetCameraMode(character.chosenPlayerPerspective);
        }

        void OnDestroy()
        {
            if (this == localPlayer)
            {
                //If the player is being destroyed, save the camera!
                CameraModeController.singleton.CameraParent = null;
                DontDestroyOnLoad(CameraModeController.singleton.camPoint);
                CameraModeController.singleton.enabled = true;
            }
        }

        public void PausePlayer()
        {
            paused = true;
            GetComponent<MovementController>().enabled = false;
            GetComponent<PlayerAnimationController>().StopAnimations();
            GetComponent<PlayerAnimationController>().enabled = false;
            CameraModeController.singleton.GetCameraController().enabled = false;
            Cursor.visible = true;
        }

        public void UnpausePlayer()
        {
            paused = false;
            GetComponent<MovementController>().enabled = true;
            GetComponent<PlayerAnimationController>().enabled = true;
            CameraModeController.singleton.GetCameraController().enabled = true;
            Cursor.visible = false;
        }

        #region slot sync

        public bool gotSlot;

        [Command]
        void CmdUpdatePlayerSlot(int newSlot)
        {
            Debug.Log("Client sent slot " + newSlot.ToString() + " for " + name);
            if(!gotSlot)
                UpdateLocalSlot(newSlot);
            RpcUpdatePlayerSlot(newSlot);
        }

        [Command]
        void CmdRequestSlots()
        {
            Debug.Log("Sending clients everything I've got!");
            foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
            {
                Player player = playerObj.GetComponent<Player>();
                if (gotSlot)
                    player.RpcUpdatePlayerSlot(player.slot);
            }
            Debug.Log("Client requests slot for " + name);
        }

        [ClientRpc]
        void RpcUpdatePlayerSlot(int newSlot)
        {
            Debug.Log("Recieved rpc slot " + newSlot.ToString() + " for " + name);
            UpdateLocalSlot(newSlot);
        }

        void UpdateLocalSlot(int value)
        {
            //If the client already has the slot, don't need to update.
            if (gotSlot)
                return;

            if (isLocalPlayer)
                return;

            slot = value;
            gotSlot = true;

            LobbyPlayerData lobbyPlayer = Networking.NetworkManager.instance.GetLobbyPlayerBySlot(slot);
            character = lobbyPlayer.character;
            name = lobbyPlayer.name;

            Debug.Log("recieved slot " + value.ToString() + " for " + name);

            SetupGraphicsModel();
        }

        #endregion
    }
}