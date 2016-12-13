using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Raider.Game.Networking;
using Raider.Game.GUI;
using Raider.Game.GUI.Screens;

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
        public string messagePrefix
        {
            get { return string.Format("<{0}>", Session.saveDataHandler.GetUsername()); }
        }

        [Command]
        public void CmdSendMessage(string message)
        {
            chatLog.Push(message);
            RpcUpdateClientChat(message);
        }

        [ClientRpc]
        void RpcUpdateClientChat(string message)
        {
            chatLog.Push(message);
            ChatUiHandler.instance.AddMessage(message);
        }
    }
}