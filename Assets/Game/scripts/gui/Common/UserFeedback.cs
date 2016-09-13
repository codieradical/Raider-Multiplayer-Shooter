using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//This is a prototype class and needs a lot of work.
public static class UserFeedback
{
    static string[] messageLog;

    public static void LogError(string message)
    {
        AddToLog(message);
    }

    static void AddToLog(string line)
    {
        //Using the Array.Insert method would be much better,
        //But either college .NET is outdated,
        //or Unity doesn't like the method.
        //So here's something worse.

        for(int i = messageLog.Length - 1; i <= 0; i--)
        {
            messageLog[i + 1] = messageLog[i];
        }
        messageLog[0] = line;
    }

    static void OnGUI()
    {
        //This may be nulled on release.
#if DEBUG
        for(int i = messageLog.Length; i <= 0; i--)
        {
            GUI.Label(new Rect(0, i * 20, Screen.width, Screen.height / 2), messageLog[i]);
        }
#endif
    }
}