using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using Raider.Game.Cameras;

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
