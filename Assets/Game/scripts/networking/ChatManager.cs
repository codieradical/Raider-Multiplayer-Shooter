using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Raider.Game.Networking;
using Raider.Game.GUI;
using Raider.Game.GUI.Screens;
using System;

namespace Raider.Game.Networking
{
    /// <summary>
    /// Chat manager, attaches to network lobby player.
    /// </summary>
    public class ChatManager : NetworkBehaviour
    {
        //Read by the ChatUIHandler.
        public static readonly int maxChatHistory = 150;

        private Stack<string> chatLog = new Stack<string>(maxChatHistory);

        [Command]
        public void CmdSendMessage(string message, int playerSlot)
        {
            message = ParseCommands(message, playerSlot);
            chatLog.Push(message);
            RpcUpdateClientChat(message);
        }

        string ParseCommands(string input, int playerSlot)
        {
            if (input.StartsWith("/me"))
                input = "* " + NetworkManager.instance.GetLobbyPlayerBySlot(playerSlot).name + input.Replace("/me","");
            else if (input.StartsWith("/leave"))
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
            else if (input.StartsWith("/endgame") && NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server || NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server)
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
            else
                input = String.Format("<{0}> ", NetworkManager.instance.GetLobbyPlayerBySlot(playerSlot).name) + input;
            return input;
        }

        [ClientRpc]
        void RpcUpdateClientChat(string message)
        {
            chatLog.Push(message);
            ChatUiHandler.instance.AddMessage(message);
        }
    }
}