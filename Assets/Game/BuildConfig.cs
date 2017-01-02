using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raider.Game
{
    public static class BuildConfig
    {
        //Major = Big releases.
        //Minor = Small releases.
        //Build = Bugfixes.
        //Revision = Test builds.

        /// <summary>
        /// Used when checking the API for updates.
        /// </summary>
        public readonly static Version VERSION_NUMBER = new Version(0, 3, 4, 2);
        /// <summary>
        /// If set to true, the Session class will attempt to communicate with the API to recieve user data.
        /// If set to false, a LocalSerialized User save data handler will be used instead.
        /// </summary>
        public const bool ONLINE_MODE = true;
        /// <summary>
        /// If set to true, serialized save data handlers will be instanced.
        /// If set to false, data will be stored in JSON.
        /// </summary>
        public const bool SERIALIZE_SAVEDATA = false;
        /// <summary>
        /// If set to true, TeamSpeak 3 binaries will be distributed with builds.
        /// If set to false, no binaries will be distributed, and VoIP Chat Managers will self-destruct.
        /// </summary>
        public const bool VOIP_ENABLED = true;
        /// <summary>
        /// The base path for local save data files.
        /// Modified on debug builds so that saves used in testing are committed to the repo.
        /// </summary>
#if DEBUG
        public readonly static string LOCAL_SAVE_PATH = Application.dataPath.Replace("/Assets", "") + "/uploadData";
#else
        public readonly static string LOCAL_SAVE_PATH = Application.persistentDataPath;
#endif
        /// <summary>
        /// Where the API can be found.
        /// localhost:3000/api/ for local testing.
        /// </summary>
        public const string API_URL = "http://localhost:3000/api";
        /// <summary>
        /// Where the Web front end can be located.
        /// Used when opening links to Register, that sort of stuff.
        /// </summary>
        public const string WEB_FRONT_URL = "";
    }
}
