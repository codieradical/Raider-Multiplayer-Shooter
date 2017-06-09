using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.Player;
using System;
using UnityEngine;

namespace Raider.Game.Networking
{
	public class LobbySetup : MonoBehaviour
    {
        [Serializable]
        public class SyncData
        {
            public string gametype;
            public string GametypeString
            {
                get { return gametype; }
                set
                {
                    gametype = value;
                    LobbySetupPane.instance.UpdatePaneData();

                    NetworkGameManager.instance.lobbySetup.SendLobbySetupUpdate();
                }
            }
            public GametypeHelper.Gametype Gametype
            {
                get
                {
                    //Replace spaces in the string with underscores, parse.
                    return GametypeHelper.instance.GetGametypeByTitle(GametypeString);
                }
                set
                {
                    GametypeString = GametypeHelper.instance.GetGametypeTitle(value);
                }
            }
            public string selectedScene;
            public string SelectedScene
            {
                get { return selectedScene; }
                set
                {
                    selectedScene = value;
                    LobbySetupPane.instance.UpdatePaneData();

                    NetworkGameManager.instance.playScene = value;

                    NetworkGameManager.instance.lobbySetup.SendLobbySetupUpdate();
                }
            }
            public string network;
            public string Network
            {
                get { return network; }
                set
                {
                    network = value;
                    LobbySetupPane.instance.UpdatePaneData();

                    NetworkGameManager.instance.lobbySetup.SendLobbySetupUpdate();
                }
            }

            public GametypeController.GameOptions gameOptions;
        }

        public SyncData syncData = new SyncData();

        //Used to send lobby setup changes over a network.
        public void SendLobbySetupUpdate()
        {
            //If the player is offline, there's nowhere to send the data...
            if(NetworkGameManager.instance.CurrentNetworkState != NetworkGameManager.NetworkState.Offline && NetworkLobbyPlayerSetup.localPlayer.playerData.PlayerSyncData.isLeader)
                NetworkLobbyPlayerSetup.localPlayer.CmdSendLobbySetup(syncData);
        }

        //Used to recieve lobby setup changes without causing a syncing loop by changing private property backends.
        public void RecieveLobbySetupUpdate(SyncData syncData)
        {
            this.syncData = syncData;

            if(LobbySetupPane.instance != null)
                LobbySetupPane.instance.UpdatePaneData();
        }
    }
}