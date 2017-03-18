using Raider.Game.GUI;
using System;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

namespace Raider.Game.Saves.System
{
    public class LocalJsonSystemSaveDataHandler : ISystemSaveDataHandler
    {
        private SystemSaveDataStructure data;
        readonly string dataPath = BuildConfig.LOCAL_SAVE_PATH + "/systemSaveData.json";

        public LocalJsonSystemSaveDataHandler()
        {
            ReloadData();
        }

        public void DefaultSettings()
        {
            data.systemSettings = new SystemSaveDataStructure.SystemSettings();
            SaveData(data);
        }

        public void DeleteData()
        {
            data = null;
            File.Delete(dataPath);
        }

        public void DeleteToken()
        {
            data.sessionToken = "";
            SaveData(data);
        }

        public SystemSaveDataStructure.SystemSettings GetSettings()
        {
            return data.systemSettings;
        }

        public string GetToken()
        {
            return data.sessionToken;
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
                data = new SystemSaveDataStructure();
                SaveData(data);
                ReloadData();
            }
            catch (IOException)
            {
                UserFeedback.LogError("Unable to create new save data due to an IO Exception.");
                UserFeedback.LogError("Sharing Violation?");
            }
        }

        public SystemSaveDataStructure GetData()
        {
            return data;
        }

        public void ReloadData()
        {
            if (File.Exists(dataPath))
            {
                try
                {
                    SystemSaveDataStructure _data = JsonUtility.FromJson<SystemSaveDataStructure>(File.ReadAllText(dataPath));
                    data = _data;
                }
                catch (SerializationException)
                {
                    UserFeedback.LogError("Failed to deserialize saveData.");
                    UserFeedback.LogError("Savedata is corrupted. Creating new file.");
                    NewData();
                }
            }
            else
            {
                NewData();
            }
        }

        public void SaveData(SystemSaveDataStructure _data)
        {
            try
            {
                File.WriteAllText(dataPath, JsonUtility.ToJson(_data));
            }
            catch (Exception ex)
            {
                //College sharing violations are hard to avoid so, this is necessary for now.
                UserFeedback.LogError("Error reading System Save Data JSON");
                Debug.LogException(ex);
                throw new FileLoadException();
            }

            ReloadData();
        }

        public void SaveSettings(SystemSaveDataStructure.SystemSettings settings)
        {
            data.systemSettings = settings;
            SaveData(data);
        }

        public void SetToken(string newToken)
        {
            data.sessionToken = newToken;
        }
    }
}
