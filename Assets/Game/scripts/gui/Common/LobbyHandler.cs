﻿using Raider.Game.GUI.Layout;
using Raider.Game.Networking;
using Raider.Game.Player;
using Raider.Game.Saves.User;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{
	public class LobbyHandler : MonoBehaviour
    {
        #region Singleton Setup

        private static List<LobbyHandler> instances = new List<LobbyHandler>();

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

        private static List<PlayerData.SyncData> players = new List<PlayerData.SyncData>();

		public void SwitchToScrollLobbyButton()
		{
			UserSaveDataStructure.UserSettings settings = Session.userSaveDataHandler.GetSettings();
			settings.LobbyDisplay = UserSaveDataStructure.UserSettings.LobbyDisplays.Scroll;
			Session.userSaveDataHandler.SaveSettings(settings, null, FailedToSaveSettingsCallback);

			SwitchToScrollLobby();
		}

		public void SwitchToSplitLobbyButton()
		{
			UserSaveDataStructure.UserSettings settings = Session.userSaveDataHandler.GetSettings();
			settings.LobbyDisplay = UserSaveDataStructure.UserSettings.LobbyDisplays.Split;
			Session.userSaveDataHandler.SaveSettings(settings, null, FailedToSaveSettingsCallback);

			SwitchToSplitLobby();
		}

        public void FailedToSaveSettingsCallback(string error)
        {
            Debug.Log("Failed to save user lobby display settings. \n" + error);
        }

        public void Start()
        {
            NetworkGameManager.instance.UpdateLobbyNameplates();

            if (Session.userSaveDataHandler == null)
                return;

            if (Session.userSaveDataHandler.GetSettings().LobbyDisplay == UserSaveDataStructure.UserSettings.LobbyDisplays.Split)
                SwitchToSplitLobby();
            else
                SwitchToScrollLobby();
        }

        public static void DestroyAllPlayers()
        {
            players = new List<PlayerData.SyncData>();

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

            if (Session.userSaveDataHandler.GetSettings().LobbyDisplay == UserSaveDataStructure.UserSettings.LobbyDisplays.Split)
            {
                if (players.Count == 9 || players.Count == 17 || players.Count == 33)
                    SwitchToSplitLobby();
            }
        }

        public static void AddPlayer(PlayerData.SyncData player)
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
                    instance.lobbyOwnerText.text = Session.userSaveDataHandler.GetUsername() + "'s Lobby";
                //I would like this to be the host's username.
                else
                {
                    foreach (PlayerData playerData in NetworkGameManager.instance.Players)
                    {
                        if (playerData.PlayerSyncData.isHost)
                        {
                            instance.lobbyOwnerText.text = playerData.PlayerSyncData.username + " is the Leader.";
                            break;
                        }
                    }
                }
            }

			if (Session.userSaveDataHandler.GetSettings ().LobbyDisplay == UserSaveDataStructure.UserSettings.LobbyDisplays.Split) {
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