using Raider.Game.GUI;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Raider.Game.Saves.System
{
    public class LocalSerializedSystemSaveDataHandler : ISystemSaveDataHandler
    {
        private SystemSaveDataStructure data;
        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        readonly string dataPath = BuildConfig.LOCAL_SAVE_PATH + "/systemSaveData.dat";

        public LocalSerializedSystemSaveDataHandler()
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
            if (File.Exists(dataPath))
            {
                FileStream file = File.Open(dataPath, FileMode.Open);
                try
                {
                    SystemSaveDataStructure _data = (SystemSaveDataStructure)binaryFormatter.Deserialize(file);
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

        public void ReloadData()
        {
            data = GetData();
        }

        public void SaveData(SystemSaveDataStructure _data)
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
