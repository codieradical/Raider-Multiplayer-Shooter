using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raider.Game.GUI
{
    public class UserFeedback : MonoBehaviour
    {
        #region Singleton Setup

        public static UserFeedback instance;

        public void Awake()
        {
            if (instance != null)
                Debug.LogAssertion("It seems that multiple User Feedback Scripts are active, breaking the singleton instance.");
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        private Queue<string> messageLog = new Queue<string>();
        private const int MAX_LOG_COUNT = 15;

        public static void LogError(string message)
        {
            AddToLog(message);

            //I need to think about this more but for now, these belong here.
            if (Scene.Scenario.InLobby)
                Networking.LobbyPlayerData.localPlayer.GetComponent<Networking.ChatManager>().SendLocalNotificationMessage(message);
            else if (Player.Player.localPlayer != null)
                Player.Player.localPlayer.GetComponent<Networking.ChatManager>().SendLocalNotificationMessage(message);
        }

        private static void AddToLog(string line)
        {
            if (instance == null)
            {
                Debug.LogError("User feedback instance is null.");
                return;
            }

            instance.messageLog.Enqueue(line);
            if (instance.messageLog.Count > MAX_LOG_COUNT)
            {
                instance.messageLog.Dequeue();
            }
        }

        void OnGUI()
        {
            UnityEngine.GUI.Label(new Rect(0, 0, Screen.width, 30), "Project Raider, 2016 Private Alpha // User Feedback");

            if (messageLog.Count > 0)
                for (int i = messageLog.Count - 1; i >= 0; i--)
                {
                    UnityEngine.GUI.Label(new Rect(0, (i * 15) + 30, Screen.width, Screen.height / 2), messageLog.ElementAt(i));
                }
        }
    }
}