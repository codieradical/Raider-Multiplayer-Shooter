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

        //This should be public get.
        public static Stack<string> chatLog = new Stack<string>(maxChatHistory);

        [Command]
        public void CmdSendMessage(string message, int playerSlot)
        {
            message = ParseCommands(message, playerSlot);
            chatLog.Push(message);
            RpcUpdateClientChat(message);
        }

        /// <summary>
        /// Sends a notification message, formatted in bold, to the server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="playerSlot">The slot containing the sender's data. Used for formatting.</param>
        [Command]
        public void CmdSendNotificationMessage(string message, int playerSlot)
        {
            message = GetFormattedUsername(playerSlot) + " " + AddBoldCode(message);
            chatLog.Push(message);
            RpcUpdateClientChat(message);
        }

        /// <summary>
        /// Sends a local chat message that only the sender can see.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendLocalNotificationMessage(string message)
        {
            message = AddBoldCode(message);
            chatLog.Push(message);
            ChatUiHandler.instance.AddMessageToFullLog(message);
            ChatUiHandler.instance.AddMessageToShortLog(message);
        }

        string ParseCommands(string input, int playerSlot)
        {
            if (input.StartsWith("/me"))
                input = "* " + GetFormattedUsername(playerSlot) + input.Replace("/me","");
            else if (input.StartsWith("/leave"))
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
            else if (input.StartsWith("/endgame") && (NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Server || NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Host))
                NetworkManager.instance.CurrentNetworkState = NetworkManager.NetworkState.Offline;
            else
                input = string.Format("<{0}> {1}", GetFormattedUsername(playerSlot), input);
            return input;
        }

        [ClientRpc]
        void RpcUpdateClientChat(string message)
        {
            //Hosts and servers already add the message to the log during the CMD.
            if(NetworkManager.instance.CurrentNetworkState == NetworkManager.NetworkState.Client)
                chatLog.Push(message);

            ChatUiHandler.instance.AddMessageToFullLog(message);
            ChatUiHandler.instance.AddMessageToShortLog(message);
        }

        /// <summary>
        /// Returns the username of the player provided, with color and bold.
        /// </summary>
        /// <param name="playerSlot"></param>
        /// <returns></returns>
        string GetFormattedUsername(int playerSlot)
        {
            LobbyPlayerData playerData = NetworkManager.instance.GetLobbyPlayerBySlot(playerSlot);
            return AddBoldCode(AddColorCode(playerData.character.armourPrimaryColor.Color, playerData.name));
        }

        static string AddColorCode(Color color, string message)
        {
            byte a = (byte)(color.a * 255);
            byte r = (byte)(color.r * 255);
            byte g = (byte)(color.g * 255);
            byte b = (byte)(color.b * 255);

            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", r, g, b, a, message);
        }

        static string AddBoldCode(string message)
        {
            return string.Format("<b>{0}</b>", message);
        }
    }
}