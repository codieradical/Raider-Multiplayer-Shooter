using System;

namespace Raider.Game.Saves.System
{
    /// <summary>
    /// Settings and other data stored locally on the device.
    /// </summary>
    [Serializable]
    public class SystemSaveDataStructure
    {
        [Serializable]
        public class SystemSettings
        {
            public bool VoIPClientEnabled = true;
        }

        public SystemSettings systemSettings = new SystemSettings();
        public string sessionToken;
    }
}
