using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Raider.Game.GUI;
using System;

namespace Raider.Game.Saves
{

    class LocalSerializedSaveDataHandler : ISaveDataHandler
    {
        [SerializeField]
        private SaveDataStructure Data;

        private BinaryFormatter bf = new BinaryFormatter();

        const string fileName = "/saveData.dat";

#if DEBUG
        readonly string dataPath =
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
                    return Data;
                }
            }
            else
            {
                NewData();
                return Data;
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
            Data = null;
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
                Data = new SaveDataStructure();
                SaveData(Data);
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
            Data = ReadData();
        }


        public void NewCharacter(SaveDataStructure.Character character)
        {
            Data.characters.Add(character);
            SaveData(Data);
        }

        public void SaveCharacter(int slot, SaveDataStructure.Character character)
        {
            Data.characters[slot] = character;
            SaveData(Data);
        }

        public SaveDataStructure.Character GetCharacter(int slot)
        {
            return Data.characters[slot];
        }

        public string GetUsername()
        {
            return Data.username;
        }

        public void SetUsername(string _username)
        {
            Data.username = _username;
            SaveData(Data);
        }

        public int CharacterCount
        {
            get
            {
                if (Data.characters == null)
                    return 0;
                else
                    return Data.characters.Count;
            }
        }

        public void DeleteCharacter(int slot)
        {
            Data.characters.RemoveAt(slot);
            SaveData(Data);
        }

        public List<SaveDataStructure.Character> GetAllCharacters()
        {
            return Data.characters;
        }

		public void DefaultSettings()
		{
			Data.settings = new SaveDataStructure.Settings();
			SaveData (Data);
		}

		public void SaveSettings(SaveDataStructure.Settings settings)
		{
			Data.settings = settings;
			SaveData (Data);
		}

        //Not sure what's going on here...
		//SaveDataStructure.Settings GetSettings()
		//{
		//	return Data.settings;
		//}

        SaveDataStructure.Settings ISaveDataHandler.GetSettings()
        {
            return Data.settings;
        }
    }
}