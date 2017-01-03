using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Raider.Game.GUI;
using System;

namespace Raider.Game.Saves.User
{

    class LocalJsonUserSaveDataHandler : IUserSaveDataHandler
    {
        private UserSaveDataStructure data;

        readonly string dataPath = BuildConfig.LOCAL_SAVE_PATH + "/userSaveData.json";

        public void Login(string username, string password, Action<bool, string> messageCallback)
        {
            ReloadData(null);
            SetUsername(username,null);

            if(messageCallback != null)
                messageCallback(true, "success");
        }

        public UserSaveDataStructure ReadData()
        {
            if (File.Exists(dataPath))
            {
                try
                {
                    UserSaveDataStructure _data = JsonUtility.FromJson<UserSaveDataStructure>(File.ReadAllText(dataPath));
                    return _data;
                }
                catch (SerializationException)
                {
                    UserFeedback.LogError("Failed to deserialize saveData.");
                    UserFeedback.LogError("Savedata is corrupted. Creating new file.");
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

        public void SaveData(UserSaveDataStructure _data, Action<bool, string> messageCallback)
        {
            try
            {
                File.WriteAllText(dataPath, JsonUtility.ToJson(data));
                if (messageCallback != null)
                    messageCallback(true, "success.");
            }
            catch (Exception ex)
            {
                //College sharing violations are hard to avoid so, this is necessary for now.
                UserFeedback.LogError("IO Exception! This may be due to a sharing violation.");
                Debug.LogException(ex);
                if (messageCallback != null)
                    messageCallback(false, "Unhandled exception saving user data.");
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
                SaveData(data,null);
                ReloadData(null);
            }
            catch(IOException)
            {
                UserFeedback.LogError("Unable to create new save data due to an IO Exception.");
                UserFeedback.LogError("Sharing Violation?");
            }
        }


        public void ReloadData(Action<bool,string> messageCallback)
        {
            if (File.Exists(dataPath))
            {
                try
                {
                    UserSaveDataStructure _data = JsonUtility.FromJson<UserSaveDataStructure>(File.ReadAllText(dataPath));
                    data = _data;
                    if(messageCallback != null)
                        messageCallback(true, "successfully read user data");
                }
                catch (SerializationException)
                {
                    UserFeedback.LogError("Failed to deserialize saveData.");
                    UserFeedback.LogError("Savedata is corrupted. Creating new file.");
                    NewData();
                    if (messageCallback != null)
                        messageCallback(true, "User data was corrupted, created new data.");
                }
            }
            else
            {
                NewData();
                messageCallback(true, "No user data found, created new data");
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
			SaveData(data, messageCallback);
		}

        public UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }
    }
}