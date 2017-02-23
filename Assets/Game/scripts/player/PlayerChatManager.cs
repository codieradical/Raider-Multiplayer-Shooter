using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Raider.Game.Networking;
using Raider.Game.GUI;
using Raider.Game.GUI.Screens;
using System;
using Raider.Game.Player;

namespace Raider.Game.Player
{
    /// <summary>
    /// Chat manager, attaches to network lobby player.
    /// </summary>
    public class PlayerChatManager : NetworkBehaviour
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
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
            else if (input.StartsWith("/endgame") && (NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Server || NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Host))
                NetworkGameManager.instance.CurrentNetworkState = NetworkGameManager.NetworkState.Offline;
            else
                input = string.Format("<{0}> {1}", GetFormattedUsername(playerSlot), input);
            return input;
        }

        [ClientRpc]
        void RpcUpdateClientChat(string message)
        {
            //Hosts and servers already add the message to the log during the CMD.
            if(NetworkGameManager.instance.CurrentNetworkState == NetworkGameManager.NetworkState.Client)
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
            PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerSlot);
            Color usernameColor = playerData.syncData.Character.armourPrimaryColor.Color;
            float H, S, V = 0f;
            Color.RGBToHSV(usernameColor, out H, out S, out V);
            usernameColor = Color.HSVToRGB(H, 0.5f, 0.5f);
            return AddBoldCode(AddColorCode(usernameColor, playerData.syncData.username));
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