using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

class LocalNonSerializedSaveDataHandler : ISaveDataHandler
{
    public string dataPath = Application.persistentDataPath + "/saveData.txt";

    public void SaveCharacter()
    {
    }
    public void LoadCharacter()
    {
    }
}
