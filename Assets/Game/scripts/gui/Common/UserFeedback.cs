using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UserFeedback : MonoBehaviour
{
    #region Singleton Setup

    public static UserFeedback instance;

    public void Awake()
    {
        if (instance != null)
            Debug.LogAssertion("It seems that multiple User Feedback Scripts are active, breaking the singleton instance.");
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    #endregion

    private Queue<string> messageLog = new Queue<string>();
    private const int MAX_LOG_COUNT = 5;

    public static void LogError(string message)
    {
        AddToLog(message);
    }

    private static void AddToLog(string line)
    {
        if (instance == null)
        {
            Debug.LogError("User feedback instance is null.");
            return;
        }

        instance.messageLog.Enqueue(line);
        if(instance.messageLog.Count > MAX_LOG_COUNT)
        {
            instance.messageLog.Dequeue();
        }
    }

    void OnGUI()
    {
        if(messageLog.Count > 1)
            for(int i = messageLog.Count - 1; i >= 0; i--)
            {
                GUI.Label(new Rect(0, i * 15, Screen.width, Screen.height / 2), messageLog.ElementAt(i));
            }

        GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), "Project Excavator, 2016 Private Alpha");
    }
}