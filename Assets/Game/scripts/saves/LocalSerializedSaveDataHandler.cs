using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

class LocalSerializedSaveDataHandler : ISaveDataHandler
{
    public string dataPath = Application.persistentDataPath + "/saveData.dat";

    public void SaveCharacter()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(dataPath, FileMode.Open);

        SaveDataStructure.Character character = new SaveDataStructure.Character();

        bf.Serialize(file, character);
        file.Close();
    }
    public void LoadCharacter()
    {
        if(File.Exists(dataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            SaveDataStructure.Character data = (SaveDataStructure.Character)bf.Deserialize(file);
            file.Close();
        }
    }
}
