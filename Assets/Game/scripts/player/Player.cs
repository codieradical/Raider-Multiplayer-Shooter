using UnityEngine;
using Raider.Game.Networking;
using UnityEngine.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;

namespace Raider.Game.Player
{
    public class Player : NetworkBehaviour
    {
        [System.Serializable]
        private class RaceGraphics
        {
            public GameObject xRaceGraphics;
            public GameObject yRaceGraphics;

            public void CheckAllGraphicsPresent()
            {
                if (xRaceGraphics == null || yRaceGraphics == null)
                    Debug.LogError("The player is missing a model prefab!!!");
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
        }

        [SerializeField]
        private RaceGraphics raceGraphics;
        public bool lockCursor = true;
        public SaveDataStructure.Character character;

        public static Player localPlayer;
        public GameObject graphicsObject;

        // Use this for initialization
        void Start()
        {
            raceGraphics.CheckAllGraphicsPresent();

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SetupGraphicsModel();

            //If the player is a client, or is playing alone, add the moving mechanics.
            if (isLocalPlayer || Networking.NetworkManager.instance.CurrentNetworkState == Networking.NetworkManager.NetworkState.Offline)
            {
                SetupLocalPlayer();
            }
        }

        void SetupGraphicsModel()
        {
            //Spawn the graphics.
            graphicsObject = Instantiate(raceGraphics.GetGraphicsByRace(character.race)) as GameObject;
            graphicsObject.transform.SetParent(this.transform, false);

            //Update the colors, emblem.
            graphicsObject.GetComponent<PlayerAppearenceController>().UpdatePlayerAppearence(transform.name, character);
        }

        void SetupLocalPlayer()
        {
            localPlayer = this;
            gameObject.AddComponent<MovementController>();
            gameObject.AddComponent<PlayerAnimationController>();
            CameraModeController.singleton.playerGameObject = gameObject;
            CameraModeController.singleton.SetCameraMode(CameraModeController.CameraModes.ThirdPerson);
        }

        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.CameraParent = null;
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
            CameraModeController.singleton.enabled = true;
        }

        public void PausePlayer()
        {
            GetComponent<MovementController>().enabled = false;
            CameraModeController.singleton.GetCameraController().enabled = false;
            Cursor.visible = true;
        }

        public void UnpausePlayer()
        {
            GetComponent<MovementController>().enabled = true;
            CameraModeController.singleton.GetCameraController().enabled = true;
            Cursor.visible = false;
        }
    }
}