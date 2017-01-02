using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Raider.Game.GUI;
using System;

namespace Raider.Game.Saves.User
{

    class LocalSerializedUserSaveDataHandler : IUserSaveDataHandler
    {
        private UserSaveDataStructure data;
        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        readonly string dataPath = BuildConfig.LOCAL_SAVE_PATH + "/userSaveData.dat";

        public void Login(string username, string password, Action<bool, string> messageCallback)
        {
            ReloadData(null);
            SetUsername(username, null);

            if (messageCallback != null)
                messageCallback(true, "success");
        }

        public UserSaveDataStructure ReadData()
        {
            return data;
        }

        public void SaveData(UserSaveDataStructure _data, Action<bool, string> messageCallback)
        {
            try
            {
                FileStream file = File.Open(dataPath, FileMode.OpenOrCreate);
                binaryFormatter.Serialize(file, _data);
                file.Close();
            }
            catch (IOException)
            {
                //College sharing violations are hard to avoid so, this is necessary for now.
                UserFeedback.LogError("IO Exception! This may be due to a sharing violation.");
                throw new FileLoadException();
            }

            ReloadData(messageCallback);
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
                data = new UserSaveDataStructure();
                SaveData(data, null);
                ReloadData(null);
            }
            catch(IOException)
            {
                UserFeedback.LogError("Unable to create new save data due to an IO Exception.");
                UserFeedback.LogError("Sharing Violation?");
            }
        }


        public void ReloadData(Action<bool, string> messageCallback)
        {
            if (File.Exists(dataPath))
            {
                FileStream file = File.Open(dataPath, FileMode.Open);
                try
                {
                    UserSaveDataStructure _data = (UserSaveDataStructure)binaryFormatter.Deserialize(file);
                    file.Close();
                    data = _data;
                    if(messageCallback != null)
                        messageCallback(true, "success");
                }
                catch (SerializationException)
                {
                    UserFeedback.LogError("Failed to deserialize saveData.");
                    UserFeedback.LogError("Savedata is corrupted. Creating new file.");
                    file.Close(); // make sure to end the file stream.
                    NewData();
                    if (messageCallback != null)
                        messageCallback(true, "success");
                }
            }
            else
            {
                NewData();
                if (messageCallback != null)
                    messageCallback(true, "success");
            }
        }


        public void NewCharacter(UserSaveDataStructure.Character character, Action<bool, string> messageCallback)
        {
            data.characters.Add(character);
            SaveData(data, messageCallback);
        }

        public void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<bool, string> messageCallback)
        {
            data.characters[slot] = character;
            SaveData(data, messageCallback);
        }

        public UserSaveDataStructure.Character GetCharacter(int slot)
        {
            return data.characters[slot];
        }

        public string GetUsername()
        {
            return data.username;
        }

        public void SetUsername(string _username, Action<bool, string> messageCallback)
        {
            data.username = _username;
            SaveData(data, messageCallback);
        }

        public int CharacterCount
        {
            get
            {
                if (data.characters == null)
                    return 0;
                else
                    return data.characters.Count;
            }
        }

        public void DeleteCharacter(int slot, Action<bool, string> messageCallback)
        {
            data.characters.RemoveAt(slot);
            SaveData(data, messageCallback);
        }

        public List<UserSaveDataStructure.Character> GetAllCharacters()
        {
            return data.characters;
        }

		public void DefaultSettings(Action<bool, string> messageCallback)
		{
            SaveSettings(new UserSaveDataStructure.UserSettings(), messageCallback);
		}

		public void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<bool, string> messageCallback)
		{
			data.userSettings = settings;
			SaveData (data, messageCallback);
		}

        public UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }
    }
}