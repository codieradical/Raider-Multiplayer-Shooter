using System;
using System.Collections.Generic;

namespace Raider.Game.Saves.User
{

    public interface IUserSaveDataHandler
    {
        void Login(string username, string password, Action<string> successCallback, Action<string> failureCallback);

        UserSaveDataStructure ReadData();
        void ReloadData(Action<string> successCallback, Action<string> failureCallback);
        void DeleteData();
        void NewData();

        void NewCharacter(UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback);
        void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback);

		void DefaultSettings(Action<string> successCallback, Action<string> failureCallback);
		void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<string> successCallback, Action<string> failureCallback);
		UserSaveDataStructure.UserSettings GetSettings();

        UserSaveDataStructure.Character GetCharacter(int slot);
        List<UserSaveDataStructure.Character> GetAllCharacters();
        string GetUsername();
        void SetUsername(string _username, Action<string> successCallback, Action<string> failureCallback);
        int CharacterCount { get; }
        void DeleteCharacter(int slot, Action<string> successCallback, Action<string> failureCallback);
    }
}