using Raider.Game.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Raider.Game.Saves.User
{

    class LocalSerializedUserSaveDataHandler : IUserSaveDataHandler
    {
        private UserSaveDataStructure data;
        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        readonly string dataPath = BuildConfig.LOCAL_SAVE_PATH + "/userSaveData.dat";

        public void Login(string username, string password, Action<string> successCallback, Action<string> failureCallback)
        {
            ReloadData(null, null);
            SetUsername(username, successCallback, failureCallback);
        }

        public UserSaveDataStructure ReadData()
        {
            return data;
        }

        public void SaveData(UserSaveDataStructure _data, Action<string> successCallback, Action<string> failureCallback)
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

            ReloadData(successCallback, failureCallback);
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
                SaveData(data, null, null);
                ReloadData(null, null);
            }
            catch(IOException)
            {
                UserFeedback.LogError("Unable to create new save data due to an IO Exception.");
                UserFeedback.LogError("Sharing Violation?");
            }
        }


        public void ReloadData(Action<string> successCallback, Action<string> failureCallback)
        {
            if (File.Exists(dataPath))
            {
                FileStream file = File.Open(dataPath, FileMode.Open);
                try
                {
                    UserSaveDataStructure _data = (UserSaveDataStructure)binaryFormatter.Deserialize(file);
                    file.Close();
                    data = _data;
                    if(successCallback != null)
                        successCallback("Read data.");
                }
                catch (SerializationException)
                {
                    UserFeedback.LogError("Failed to deserialize saveData.");
                    UserFeedback.LogError("Savedata is corrupted. Creating new file.");
                    file.Close(); // make sure to end the file stream.
                    NewData();
                    if (successCallback != null)
                        successCallback("Recreated data.");
                }
            }
            else
            {
                NewData();
                if (successCallback != null)
                    successCallback("Recreated.");
            }

            foreach (Action method in Session.dataReloadCallbacks)
            {
                if (method != null)
                    method();
                else
                    Session.dataReloadCallbacks.Remove(method);
            }
        }


        public void NewCharacter(UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters.Add(character);
            SaveData(data, successCallback, failureCallback);
        }

        public void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters[slot] = character;
            SaveData(data, successCallback, failureCallback);
        }

        public UserSaveDataStructure.Character GetCharacter(int slot)
        {
            return data.characters[slot];
        }

        public string GetUsername()
        {
            return data.username;
        }

        public void SetUsername(string _username, Action<string> successCallback, Action<string> failureCallback)
        {
            data.username = _username;
            SaveData(data, successCallback, failureCallback);
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

        public void DeleteCharacter(int slot, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters.RemoveAt(slot);
            SaveData(data, successCallback, failureCallback);
        }

        public List<UserSaveDataStructure.Character> GetAllCharacters()
        {
            return data.characters;
        }

		public void DefaultSettings(Action<string> successCallback, Action<string> failureCallback)
		{
            SaveSettings(new UserSaveDataStructure.UserSettings(), successCallback, failureCallback);
		}

		public void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<string> successCallback, Action<string> failureCallback)
        {
			data.userSettings = settings;
			SaveData (data, successCallback, failureCallback);
		}

        public UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }
    }
}