using Raider.Game.GUI.Components;
using Raider.Game.Networking;
using Raider.Game.Scene;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
	public class MatchmakingHandler : MonoBehaviour {

        public List<NetworkLobbyBroadcastData> LastFrameLobbies = new List<NetworkLobbyBroadcastData>();

        private void Update()
        {
            List<NetworkLobbyBroadcastData> thisFrameLobbies = NetworkGameManager.instance.NetworkDiscovery.DiscoveredLobbies;
            if (thisFrameLobbies.Count != LastFrameLobbies.Count)
                RecreateLobbyOptions(thisFrameLobbies);
            else if (thisFrameLobbies.All(LastFrameLobbies.Contains))
                RecreateLobbyOptions(thisFrameLobbies);

            LastFrameLobbies = thisFrameLobbies;
        }

        public GameObject paneSecondary;
        public GameObject paneTertiary;

        public GameObject paneSecondaryPrefab;
        public GameObject paneTertiaryPrefab;

        void RecreateLobbyOptions(List<NetworkLobbyBroadcastData> lobbies)
        {
            DestroyAllLobbies();
            if (lobbies == null)
                return;
            foreach(NetworkLobbyBroadcastData lobby in lobbies)
            {
                AddLobby(lobby);
            }
        }

        void DestroyAllLobbies()
        {
            foreach(Transform transform in paneSecondary.transform)
            {
                Destroy(transform.gameObject);
            }
            foreach (Transform transform in paneTertiary.transform)
            {
                Destroy(transform.gameObject);
            }
        }

        void AddLobby(NetworkLobbyBroadcastData lobby)
        {
            AddLobbyToGui(new OptionsPaneOption.OptionsPaneContents(lobby.GetTitle(), lobby.GetDescription(), Scenario.GetMapImage(lobby.map)));
        }

        void AddLobby(string ip)
        {
            AddLobbyToGui(new OptionsPaneOption.OptionsPaneContents(ip + " - ", "Debug on Debug :^)", Scenario.GetMapImage("Salvation")));
        }

        void AddLobbyToGui(OptionsPaneOption.OptionsPaneContents optionData)
        {
            GameObject newOption = Instantiate(paneSecondaryPrefab) as GameObject;
            newOption.GetComponent<MatchmakingOptionsPaneOption>().optionData = optionData;
            newOption.transform.SetParent(paneSecondary.transform);
        }

        public void OptionClicked(string optionName)
        {
            DirectConnect(optionName.Remove(optionName.IndexOf(" - "), optionName.Length - optionName.IndexOf(" - ")));
        }

        public void OptionHover(OptionsPaneOption.OptionsPaneContents hoveredOption)
        {
            GameObject newDescription = Instantiate(paneTertiaryPrefab) as GameObject;

            newDescription.transform.SetParent(paneTertiary.transform, false);

            //If there's an image available, show it.
            //Else, set the image scale to 0.
            //It's important to keep the image, as the description is anchored to the bottom.
            if (hoveredOption.image != null)
                newDescription.transform.Find("Image").GetComponent<Image>().sprite = hoveredOption.image;
            else
            {
                RectTransform imageRect = newDescription.transform.Find("Image").GetComponent<RectTransform>();
                Vector2 newSize = new Vector3(imageRect.sizeDelta.x, 0);
                imageRect.sizeDelta = newSize;
            }
            newDescription.transform.Find("Image").Find("Text").GetComponent<Text>().text = hoveredOption.description;
        }

        public void OptionStopHover()
        {
            //When the cursor is not hovering an option, destroy the description in pane 3.
            foreach (Transform child in paneTertiary.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void BackOut()
        {
            NetworkGameManager.instance.NetworkDiscovery.StopBroadcast();
            MenuManager.instance.ShowMenu(MainmenuController.instance.MainMenuScreen.GetComponent<Menu>());
            GametypeButtons.instance.ShowButtons();
        }

        public void DirectConnect(Text ipTxt)
        {
            DirectConnect(ipTxt.text);
        }

        public void DirectConnect(string ipTxt)
        {
            if(Session.ActiveCharacter == null)
            {
                Debug.LogWarning("User attempted to join a server with no character selected!");
            }
            if(NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline)
            {
                Debug.LogWarning("User attempted to join a server while already in a server!");
            }

            if (ipTxt.Contains(':'))
            {

                string[] ipAndPort = ipTxt.Split(':');
                int port;
                if (int.TryParse(ipAndPort[1], out port))
                {
                    NetworkGameManager.instance.networkAddress = ipAndPort[0];
                    NetworkGameManager.instance.networkPort = port;
                }
                else
                {
                    UserFeedback.LogError("Failed to parse port '" + ipAndPort[1] + "'.");
                }
            }
            else
            {
                NetworkGameManager.instance.networkAddress = ipTxt;
            }

            NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Client;
            //If the player sucessfully joined a game...
            if(NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
            {
                NetworkGameManager.instance.NetworkDiscovery.StopBroadcast();
                //Grab the lobby details.
                GametypeButtons.instance.HideButtons();
                MenuManager.instance.ShowMenu(MainmenuController.instance.MainMenuScreen.GetComponent<Menu>());
                LobbySetupPane.instance.OpenPane();
            }
        }
    }
}