using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//This is a prototype class and needs a lot of work.
public static class UserFeedback //: MonoBehaviour
{
    static Queue<string> messageLog = new Queue<string>();
    private static int maxLogCount = 5;

    public static void LogError(string message)
    {
        AddToLog(message);
    }

    static void AddToLog(string line)
    {
        messageLog.Enqueue(line);
        if(messageLog.Count > maxLogCount)
        {
            messageLog.Dequeue();
        }
    }

    static void OnGUI()
    {
        //This may be nulled on release.
#if DEBUG
        for(int i = messageLog.Count; i <= 0; i--)
        {
            GUI.Label(new Rect(0, i * 20, Screen.width, Screen.height / 2), messageLog.ElementAt(i));
        }
#endif
    }
}