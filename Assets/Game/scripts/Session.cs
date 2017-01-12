using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using Raider.Game;
using Raider.Game.Saves.User;
using System;
using Raider.Game.Saves.System;
using System.Collections.Generic;

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

        public static List<Action> dataReloadCallbacks = new List<Action>();
        public static void AddReloadHook(Action method)
        {
            if (method == null)
                return;
            dataReloadCallbacks.Add(method);
        }

        /// <summary>
        /// Initialized the SaveDataHandler.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        public static void Login(string username, string password, Action<string> successCallback, Action<string> failureCallback)
        {
            //Initialize userSaveDataHandler
            if (BuildConfig.ONLINE_MODE)
                userSaveDataHandler = new MongoUserSaveDataHandler();
            else if (BuildConfig.SERIALIZE_SAVEDATA)
                userSaveDataHandler = new LocalSerializedUserSaveDataHandler();
            else
                userSaveDataHandler = new LocalJsonUserSaveDataHandler();

            userSaveDataHandler.Login(username, password, successCallback, failureCallback);
        }

        public static void Logout()
        {
            if (ActiveCharacter != null)
            {
                userSaveDataHandler.SaveCharacter(activeCharacterSlot, ActiveCharacter, null, LogoutFailedToSaveCallback);
                DeselectCharacter();
            }
            userSaveDataHandler = null;
        }

        public static void LogoutFailedToSaveCallback(string error)
        {
            Debug.LogError("Failed to save character!\n" + error);
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

        public static void SaveActiveCharacter(Action<string> successCallback, Action<string> failureCallback)
        {
            userSaveDataHandler.SaveCharacter(activeCharacterSlot, ActiveCharacter, successCallback, failureCallback);
        }
    }
}