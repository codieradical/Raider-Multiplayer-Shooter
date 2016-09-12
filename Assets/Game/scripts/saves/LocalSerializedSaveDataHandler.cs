using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

class LocalSerializedSaveDataHandler : ISaveDataHandler
{
    [SerializeField]
    public SaveDataStructure data { get; private set; }

    private BinaryFormatter bf = new BinaryFormatter();

    public const string fileName = "/saveData.dat";
    public readonly string dataPath = Application.persistentDataPath + fileName;

    public LocalSerializedSaveDataHandler()
    {
        ReloadData();
    }

    public SaveDataStructure ReadData()
    {
        if (File.Exists(dataPath))
        {
            FileStream file = File.Open(dataPath, FileMode.Open);
            SaveDataStructure _data = (SaveDataStructure)bf.Deserialize(file);
            file.Close();
            return _data;
        }
        else
        {
            SaveData(new SaveDataStructure());
            return ReadData();
        }
    }

    public void SaveData(SaveDataStructure _data)
    {
        FileStream file = File.Open(dataPath, FileMode.OpenOrCreate);
        bf.Serialize(file, _data);
        file.Close();

        ReloadData();
    }

    public void ReloadData()
    {
        data = ReadData();
    }


    public void NewCharacter(SaveDataStructure.Character character)
    {
        data.characters.Add(character);
        SaveData(data);
    }

    public void SaveCharacter(int slot, SaveDataStructure.Character character)
    {
        data.characters[slot] = character;
        SaveData(data);
    }

    public SaveDataStructure.Character GetCharacter(int slot)
    {
        return data.characters[slot];
    }

    public string GetUsername()
    {
        return data.username;
    }

    public void SetUsername(string _username)
    {
        data.username = _username;
        SaveData(data);
    }

    public int characterCount
    {
        get
        {
            if (data.characters == null)
                return 0;
            else
                return data.characters.Count;
        }
    }

    public void DeleteCharacter(int slot)
    {
        data.characters.RemoveAt(slot);
        SaveData(data);
    }

    public List<SaveDataStructure.Character> GetAllCharacters()
    {
        return data.characters;
    }
}
