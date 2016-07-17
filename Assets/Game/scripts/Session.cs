using UnityEngine;
using System.Collections;

public class Session : MonoBehaviour {
    public ISaveDataHandler saveDataHandler;
    public bool online = false;
    public string username;

    public void Login(string _username)
    {
        username = _username;
    }

    public void InitializeSaveDataHandler()
    {
        if (online)
        {
            saveDataHandler = new MongoSaveDataHandler();
        }
        else
        {
            saveDataHandler = new LocalSerializedSaveDataHandler();
        }
    }
}
