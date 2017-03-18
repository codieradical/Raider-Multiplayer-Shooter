namespace Raider.Game.Saves.System
{
    public interface ISystemSaveDataHandler
    {
        SystemSaveDataStructure GetData();
        void SaveData(SystemSaveDataStructure _data);
        void ReloadData();
        void DeleteData();
        void NewData();

        string GetToken();
        void SetToken(string newToken);
        void DeleteToken();

        void DefaultSettings();
        void SaveSettings(SystemSaveDataStructure.SystemSettings settings);
        SystemSaveDataStructure.SystemSettings GetSettings();
    }
}
