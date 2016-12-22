using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Raider.Game.Saves;
using Raider.Game.GUI.Layout;
using Raider.Game.Networking;
using Raider.Game.Player;

namespace Raider.Game.GUI.Components
{
    public class LobbyHandler : MonoBehaviour
    {
        #region Singleton Setup

        private static List<LobbyHandler> instances;

        void Awake()
        {
            if (instances == null)
                instances = new List<LobbyHandler>();
            instances.Add(this);
        }

        void OnDestroy()
        {
            instances.Remove(this);
        }

        #endregion

        /// <summary>
        /// This prefab needs to be available on at least one lobby handler per scene.
        /// </summary>

		[Header("Header Elements")]
		public GameObject headerPanelGameObject;
		public Text lobbyStateText;
		public Text lobbyOwnerText;
		public Button scrollButton;
		public Button splitButton;

		[Header("Nameplate Prefabs")]
		public GameObject standardNameplatePrefab;
		public GameObject sixteenPlayerNameplatePrefab;
		public GameObject thirtyTwoPlayerNameplatePrefab;
		public GameObject standardLoadingNameplatePrefab;
		public GameObject sixteenLoadingNameplatePrefab;
		public GameObject thirtyTwoLoadingNameplatePrefab;

		[Header("Lobby Containers")]
		public GameObject scrollLobbyContainer;
		public GameObject sixteenPlayerLobbyContainer;
		public GameObject thirtyTwoPlayerLobbyContainer;

		[Header("Lobby Player Containers")]
		public GameObject scrollLobbyPlayerContainer;
		public GameObject sixteenPlayerLobbyPlayerContainer;
		public GameObject[] thirtyTwoPlayerLobbyPlayerContainer;

        private static List<PlayerNameplate> players = new List<PlayerNameplate>();

        public struct PlayerNameplate
        {
            public PlayerNameplate(string _username, bool _leader, bool _speaking, bool _canspeak, SaveDataStructure.Character _character)
            {
                username = _username;
                leader = _leader;
                speaking = _speaking;
                canspeak = _canspeak;
                character = _character;
            }

            public string username;
            public bool leader;
            public bool speaking;
            public bool canspeak;

            public SaveDataStructure.Character character;
        }

		public void SwitchToScrollLobbyButton()
		{
			SaveDataStructure.Settings settings = Session.saveDataHandler.GetSettings();
			settings.lobbyDisplay = SaveDataStructure.Settings.LobbyDisplay.Scroll;
			Session.saveDataHandler.SaveSettings(settings);

			SwitchToScrollLobby();
		}

		public void SwitchToSplitLobbyButton()
		{
			SaveDataStructure.Settings settings = Session.saveDataHandler.GetSettings();
			settings.lobbyDisplay = SaveDataStructure.Settings.LobbyDisplay.Split;
			Session.saveDataHandler.SaveSettings(settings);

			SwitchToSplitLobby();
		}

        public static void DestroyAllPlayers()
        {
            players = new List<PlayerNameplate>();

            foreach(LobbyHandler instance in instances)
            {
				foreach (Transform nameplate in instance.scrollLobbyPlayerContainer.transform)
                {
                    Destroy(nameplate.gameObject);
                }
				foreach (Transform nameplate in instance.sixteenPlayerLobbyPlayerContainer.transform)
				{
					Destroy(nameplate.gameObject);
				}
				foreach (Transform nameplate in instance.thirtyTwoPlayerLobbyPlayerContainer[0].transform)
				{
					Destroy(nameplate.gameObject);
				}
				foreach (Transform nameplate in instance.thirtyTwoPlayerLobbyPlayerContainer[1].transform)
				{
					Destroy(nameplate.gameObject);
				}

                if (players.Count >= NetworkGameManager.instance.maxPlayers)
                    instance.lobbyStateText.text = "full [" + players.Count.ToString() + "/" + NetworkGameManager.instance.maxPlayers.ToString() + "]";
                else
                    instance.lobbyStateText.text = "joinable [" + players.Count.ToString() + "/" + NetworkGameManager.instance.maxPlayers.ToString() + "]";
            }
        }

        public static void AddLoadingPlayer()
        {
            foreach(LobbyHandler instance in instances)
            {
				//Check that loading prefabs are present.
				if (instance.standardLoadingNameplatePrefab == null || instance.sixteenLoadingNameplatePrefab == null || instance.thirtyTwoLoadingNameplatePrefab == null)
                {
                    Debug.Log("A lobby handler is missing a nameplate prefab.");
                    Debug.LogAssertion("Please add the prefab to any lobby in the scene.");
                    throw new MissingFieldException();
                }

				//8 players...
				GameObject nameplate8 = Instantiate(instance.standardLoadingNameplatePrefab);
				nameplate8.GetComponent<PreferredSizeOverride>().providedGameObject = instance.headerPanelGameObject;
				nameplate8.GetComponent<SizeOverride>().providedGameObject = instance.headerPanelGameObject;
				nameplate8.transform.SetParent(instance.scrollLobbyPlayerContainer.transform, false);

				//16 players...
				GameObject nameplate16 = Instantiate(instance.sixteenLoadingNameplatePrefab);
				nameplate16.GetComponent<PreferredSizeOverride>().providedGameObject = instance.headerPanelGameObject;
				nameplate16.GetComponent<SizeOverride>().providedGameObject = instance.headerPanelGameObject;
				nameplate16.transform.SetParent(instance.sixteenPlayerLobbyPlayerContainer.transform, false);

				//32 players...
				GameObject nameplate32 = Instantiate(instance.thirtyTwoLoadingNameplatePrefab);
				nameplate32.GetComponent<PreferredSizeOverride>().providedGameObject = instance.headerPanelGameObject;
				nameplate32.GetComponent<SizeOverride>().providedGameObject = instance.headerPanelGameObject;

				if (players.Count < 16)
					nameplate32.transform.SetParent (instance.thirtyTwoPlayerLobbyPlayerContainer[0].transform, false);
				else
					nameplate32.transform.SetParent (instance.thirtyTwoPlayerLobbyPlayerContainer[1].transform, false);
            }

            if (Session.saveDataHandler.GetSettings().lobbyDisplay == SaveDataStructure.Settings.LobbyDisplay.Split)
            {
                if (players.Count == 9 || players.Count == 17 || players.Count == 33)
                    SwitchToSplitLobby();
            }
        }

        public static void AddPlayer(PlayerNameplate player)
        {
            players.Add(player);

            foreach (LobbyHandler instance in instances)
            {
				if (instance.standardNameplatePrefab == null || instance.sixteenPlayerNameplatePrefab == null || instance.thirtyTwoPlayerNameplatePrefab == null)
                {
                    Debug.Log("A lobby handler is missing a nameplate prefab.");
                    Debug.LogAssertion("Please add the prefab to any lobby in the scene.");
                    throw new MissingFieldException();
                }

				GameObject nameplate8 = Instantiate(instance.standardNameplatePrefab);
				nameplate8.GetComponent<LobbyNameplateHandler> ().SetupNameplate (player, instance.headerPanelGameObject, instance.scrollLobbyPlayerContainer);

				GameObject nameplate16 = Instantiate(instance.sixteenPlayerNameplatePrefab);
				nameplate16.GetComponent<LobbyNameplateHandler> ().SetupNameplate (player, instance.headerPanelGameObject, instance.sixteenPlayerLobbyPlayerContainer);

				GameObject nameplate32 = Instantiate(instance.thirtyTwoPlayerNameplatePrefab);
				if(players.Count < 17)
					nameplate32.GetComponent<LobbyNameplateHandler> ().SetupNameplate (player, instance.headerPanelGameObject, instance.thirtyTwoPlayerLobbyPlayerContainer[0]);
				else
					nameplate32.GetComponent<LobbyNameplateHandler> ().SetupNameplate (player, instance.headerPanelGameObject, instance.thirtyTwoPlayerLobbyPlayerContainer[1]);

                if (players.Count >= NetworkGameManager.instance.maxPlayers)
                    instance.lobbyStateText.text = "full [" + players.Count.ToString() + "/" + NetworkGameManager.instance.maxPlayers.ToString() + "]";
                else
                    instance.lobbyStateText.text = "joinable [" + players.Count.ToString() + "/" + NetworkGameManager.instance.maxPlayers.ToString() + "]";

                if (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Offline)
                    instance.lobbyOwnerText.text = Session.saveDataHandler.GetUsername() + "'s Lobby";
                //I would like this to be the host's username.
                else
                {
                    foreach (PlayerData playerData in NetworkGameManager.instance.Players)
                    {
                        if (playerData.isHost)
                        {
                            instance.lobbyOwnerText.text = playerData.name + " is the Leader.";
                            break;
                        }
                    }
                }
            }

			if (Session.saveDataHandler.GetSettings ().lobbyDisplay == SaveDataStructure.Settings.LobbyDisplay.Split) {
				if (players.Count == 9 || players.Count == 17 || players.Count == 33)
					SwitchToSplitLobby ();
			}
        }

		public static void SwitchToScrollLobby()
		{
			foreach (LobbyHandler instance in instances) {
				instance.sixteenPlayerLobbyContainer.SetActive(false);
				instance.thirtyTwoPlayerLobbyContainer.SetActive(false);
				instance.scrollLobbyContainer.SetActive(true);
                instance.splitButton.gameObject.SetActive(true);
                instance.scrollButton.gameObject.SetActive(false);
			}
		}

		public static void SwitchToSplitLobby()
		{
			foreach (LobbyHandler instance in instances) {
				instance.scrollLobbyContainer.SetActive (false);
				instance.sixteenPlayerLobbyContainer.SetActive(false);
				instance.thirtyTwoPlayerLobbyContainer.SetActive (false);
                instance.splitButton.gameObject.SetActive(false);
                instance.scrollButton.gameObject.SetActive(true);

                if (players.Count <= 8)
                    instance.scrollLobbyContainer.SetActive(true);
                else if (players.Count <= 16)
                    instance.sixteenPlayerLobbyContainer.SetActive(true);
                else if (players.Count <= 32)
                    instance.thirtyTwoPlayerLobbyContainer.SetActive(true);
                else
                    instance.scrollLobbyContainer.SetActive(true);
			}
		}
    }
}