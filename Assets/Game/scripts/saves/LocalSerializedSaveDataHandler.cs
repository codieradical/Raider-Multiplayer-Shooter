using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Raider.Game.GUI;

namespace Raider.Game.Saves
{

    class LocalSerializedSaveDataHandler : ISaveDataHandler
    {
        [SerializeField]
        public SaveDataStructure data { get; private set; }

        private BinaryFormatter bf = new BinaryFormatter();

        public const string fileName = "/saveData.dat";

#if DEBUG
        public readonly string dataPath =
        Application.dataPath.Replace("/Assets", "") + "/uploadData" + fileName;
#else
    public readonly string dataPath = Application.persistentDataPath + fileName;
#endif


        public LocalSerializedSaveDataHandler()
        {
            ReloadData();
        }

        public SaveDataStructure ReadData()
        {
            if (File.Exists(dataPath))
            {
                FileStream file = File.Open(dataPath, FileMode.Open);
                try
                {
                    SaveDataStructure _data = (SaveDataStructure)bf.Deserialize(file);
                    file.Close();
                    return _data;
                }
                catch (SerializationException)
                {
                    UserFeedback.LogError("Failed to deserialize saveData.");
                    UserFeedback.LogError("Savedata is corrupted. Creating new file.");
                    file.Close(); // make sure to end the file stream.
                    NewData();
                    return data;
                }
            }
            else
            {
                NewData();
                return data;
            }
        }

        public void SaveData(SaveDataStructure _data)
        {
            try
            {
                FileStream file = File.Open(dataPath, FileMode.OpenOrCreate);
                bf.Serialize(file, _data);
                file.Close();
            }
            catch (IOException)
            {
                //College sharing violations are hard to avoid so, this is necessary for now.
                UserFeedback.LogError("IO Exception! This may be due to a sharing violation.");
                throw new FileLoadException();
            }

            ReloadData();
        }

        public void DeleteData()
        {
            data = null;
            File.Delete(dataPath);
        }

        public void NewData()
        {
            try
            {
                if (File.Exists(dataPath))
                    File.Delete(dataPath);
                else if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dataPath));

                File.Create(dataPath).Dispose();
                data = new SaveDataStructure();
                SaveData(data);
                ReloadData();
            }
            catch(IOException)
            {
                UserFeedback.LogError("Unable to create new save data due to an IO Exception.");
                UserFeedback.LogError("Sharing Violation?");
            }
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
}