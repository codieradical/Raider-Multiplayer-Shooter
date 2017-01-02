using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using Raider.Game;
using Raider.Game.Saves.User;
using System;
using Raider.Game.Saves.System;

namespace Raider
{
    public static class Session
    {
        static Session()
        {
            if (BuildConfig.SERIALIZE_SAVEDATA)
                systemSaveDataHandler = new LocalSerializedSystemSaveDataHandler();
            else
                systemSaveDataHandler = new LocalJsonSystemSaveDataHandler();
        }

        //Debug:
        //Is this an online build?
        public static bool rememberMe = false;

        public static ISystemSaveDataHandler systemSaveDataHandler;
        public static IUserSaveDataHandler userSaveDataHandler;
        public static UserSaveDataStructure.Character ActiveCharacter { get; private set; }
        private static int activeCharacterSlot = -1;

        /// <summary>
        /// Initialized the SaveDataHandler.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        public static void Login(string username, string password, Action<bool, string> callback)
        {
            //Initialize userSaveDataHandler
            if (BuildConfig.ONLINE_MODE)
                userSaveDataHandler = new MongoUserSaveDataHandler();
            else if (BuildConfig.SERIALIZE_SAVEDATA)
                userSaveDataHandler = new LocalSerializedUserSaveDataHandler();
            else
                userSaveDataHandler = new LocalJsonUserSaveDataHandler();

            userSaveDataHandler.Login(username, password, callback);
            //userSaveDataHandler.lo
        }

        public static void Logout()
        {
            if (ActiveCharacter != null)
            {
                userSaveDataHandler.SaveCharacter(activeCharacterSlot, ActiveCharacter, LogoutSaveCallback);
                DeselectCharacter();
            }
            userSaveDataHandler = null;
        }

        public static void LogoutSaveCallback(bool success, string message)
        {
            if(!success)
            {
                Debug.LogError("Failed to save character!\n" + message);
            }
        }

        public static void SelectCharacter(int slot)
        {
            ActiveCharacter = userSaveDataHandler.GetCharacter(slot);
            activeCharacterSlot = slot;
        }

        public static void DeselectCharacter()
        {
            ActiveCharacter = null;
            activeCharacterSlot = -1;
        }

        public static void SaveActiveCharacter(Action<bool, string> callback)
        {
            userSaveDataHandler.SaveCharacter(activeCharacterSlot, ActiveCharacter, callback);
        }
    }
}