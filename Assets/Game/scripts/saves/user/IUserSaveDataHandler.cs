using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Raider.Game.Saves.User
{

    public interface IUserSaveDataHandler
    {
        void Login(string username, string password, Action<bool, string> messageCallback);

        UserSaveDataStructure ReadData();
        void SaveData(UserSaveDataStructure _data, Action<bool, string> messageCallback);
        void ReloadData(Action<bool, string> messageCallback);
        void DeleteData();
        void NewData();

        void NewCharacter(UserSaveDataStructure.Character character, Action<bool, string> messageCallback);
        void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<bool, string> messageCallback);

		void DefaultSettings(Action<bool, string> messageCallback);
		void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<bool, string> messageCallback);
		UserSaveDataStructure.UserSettings GetSettings();

        UserSaveDataStructure.Character GetCharacter(int slot);
        List<UserSaveDataStructure.Character> GetAllCharacters();
        string GetUsername();
        void SetUsername(string _username, Action<bool, string> messageCallback);
        int CharacterCount { get; }
        void DeleteCharacter(int slot, Action<bool, string> messageCallback);
    }
}