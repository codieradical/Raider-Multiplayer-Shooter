using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raider.Game.Saves.System;
using Raider.Game.Saves.User;

namespace Raider.Test.Saves.System
{
    public class MockSystemSaveDataHandler : ISystemSaveDataHandler
    {
        SystemSaveDataStructure data;

        public MockSystemSaveDataHandler()
        {
            data = new SystemSaveDataStructure();
        }

        public SystemSaveDataStructure GetData()
        {
            return data;
        }
        public void SaveData(SystemSaveDataStructure _data)
        {
            data = _data;
        }
        public void ReloadData()
        {

        }
        public void DeleteData()
        {
            data = null;
        }
        public void NewData()
        {
            data = new SystemSaveDataStructure();
        }

        public string GetToken()
        {
            throw new NotImplementedException();
        }
        public void SetToken(string newToken)
        {
            throw new NotImplementedException();
        }
        public void DeleteToken()
        {
            throw new NotImplementedException();
        }

        public void DefaultSettings()
        {
            data = new SystemSaveDataStructure();
        }
        public void SaveSettings(SystemSaveDataStructure.SystemSettings settings)
        {
            data.systemSettings = settings;
        }
        public SystemSaveDataStructure.SystemSettings GetSettings()
        {
            return data.systemSettings;
        }

    }
}
