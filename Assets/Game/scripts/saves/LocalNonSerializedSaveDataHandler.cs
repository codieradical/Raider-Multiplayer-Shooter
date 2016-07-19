using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

class LocalNonSerializedSaveDataHandler : ISaveDataHandler
{
    public string dataPath = Application.persistentDataPath + "/saveData.txt";

    public SaveDataStructure ReadData() { return null; }
    public void SaveCharacter(int slot, SaveDataStructure.Character character) { }
    public SaveDataStructure.Character LoadCharacter(int slot) { return null; }
    public void SaveData(SaveDataStructure _data) { }
    public void ReloadData() { }
    public string GetUsername() { return null; }
    public void SetUsername(string _username) { }
}
