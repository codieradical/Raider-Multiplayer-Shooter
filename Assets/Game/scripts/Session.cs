using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using System;

namespace Raider
{
    public static class Session
    {
        //Debug:
        //Is this an online build?
        public static bool onlineMode = true;
        public static bool rememberMe = false;

        public static ISaveDataHandler saveDataHandler;
        public static SaveDataStructure.Character ActiveCharacter { get; private set; }
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
            if (onlineMode)
                saveDataHandler = new MongoSaveDataHandler(username, password, callback);
            else
                saveDataHandler = new LocalSerializedSaveDataHandler(username, password, callback);
        }

        public static void Logout()
        {
            if (ActiveCharacter != null)
            {
                saveDataHandler.SaveCharacter(activeCharacterSlot, ActiveCharacter);
                DeselectCharacter();
            }
            saveDataHandler = null;
        }

        public static void SelectCharacter(int slot)
        {
            ActiveCharacter = saveDataHandler.GetCharacter(slot);
            activeCharacterSlot = slot;
        }

        public static void DeselectCharacter()
        {
            ActiveCharacter = null;
            activeCharacterSlot = -1;
        }

        public static void SaveActiveCharacter()
        {
            saveDataHandler.SaveCharacter(activeCharacterSlot, ActiveCharacter);
        }
    }
}