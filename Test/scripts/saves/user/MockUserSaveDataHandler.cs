using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raider.Game.Saves.User;

namespace Raider.Test.Saves.User
{
    public class MockUserSaveDataHandler
    {
        UserSaveDataStructure data = new UserSaveDataStructure();

        string username;
        void Login(string username, string password, Action<string> successCallback, Action<string> failureCallback)
        {
            this.username = username;
        }

        UserSaveDataStructure ReadData()
        {
            return data;
        }

        void ReloadData(Action<string> successCallback, Action<string> failureCallback)
        {
            throw new NotImplementedException();
        }
        void DeleteData()
        {
            data = null;
        }
        void NewData()
        {
            data = new UserSaveDataStructure();
        }

        void NewCharacter(UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters.Add(character);
        }
        void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters[slot] = character;
        }

        void DefaultSettings(Action<string> successCallback, Action<string> failureCallback)
        {
            data.userSettings = new UserSaveDataStructure.UserSettings();
        }
        void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<string> successCallback, Action<string> failureCallback)
        {
            data.userSettings = settings;
        }
        UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }

        UserSaveDataStructure.Character GetCharacter(int slot)
        {
            return data.characters[slot];
        }
        List<UserSaveDataStructure.Character> GetAllCharacters()
        {
            return data.characters;
        }
        string GetUsername()
        {
            return username;
        }
        void SetUsername(string _username, Action<string> successCallback, Action<string> failureCallback)
        {
            username = _username;
        }
        int CharacterCount { get { return data.characters.Count; } }
        void DeleteCharacter(int slot, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters.RemoveAt(slot);
        }

    }
}
