using System;
using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.Saves.User
{

    /// <summary>
    /// The GameObject Save Data Handler is volatile! 
    /// It's used for temporary storage on a script attached to a game object.
    /// 
    /// A plugin should be built to help with save data.
    /// </summary>
    public class GameObjectUserSaveDataHandler : MonoBehaviour, IUserSaveDataHandler
    {
        #region Singleton Setup

        //I think I can refactor this,
        //because MonoBehaviours can't be constructed,
        //I've had to make a kinda fake constructor method.
        //Maybe an instanciatable inheriting class would work.
        public static GameObjectUserSaveDataHandler CreateGameObjectSaveDataHandler()
        {
            GameObject hostObject = new GameObject();
            hostObject.AddComponent<GameObjectUserSaveDataHandler>();
            instance.data = new UserSaveDataStructure();
            return instance;
        }

        public static GameObjectUserSaveDataHandler instance;

        public void Awake()
        {
            if (instance != null)
                Debug.LogWarning("Multiple objects are holding save data. Ignoring additional.");
            else
                instance = this;
            parentObject = gameObject;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public UserSaveDataStructure data;
        public GameObject parentObject;

        public UserSaveDataStructure ReadData()
        {
            return data;
        }

        public void SaveData(UserSaveDataStructure _data, Action<string> successCallback, Action<string> failureCallback)
        {
            if (successCallback != null)
                successCallback("Success");
            //Game object data is non-persistant.
        }

        public void ReloadData(Action<string> successCallback, Action<string> failureCallback)
        {
            if (successCallback != null)
                successCallback("Success");
            //Game object data is non-persistant.

            foreach(Action method in Session.dataReloadCallbacks)
            {
                if (method != null)
                    method();
                else
                    Session.dataReloadCallbacks.Remove(method);
            }
        }

        public void DeleteData()
        {
            data = null;
        }

        public void NewData()
        {
            data = new UserSaveDataStructure();
        }

        public void NewCharacter(UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters.Add(character);
            if (successCallback != null)
                successCallback("Success");
        }

        public void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters[slot] = character;
            if (successCallback != null)
                successCallback("Success");
        }

        public UserSaveDataStructure.Character GetCharacter(int slot)
        {
            return data.characters[slot];
        }

        public List<UserSaveDataStructure.Character> GetAllCharacters()
        {
            return data.characters;
        }

        public string GetUsername()
        {
            return data.username;
        }

        public void SetUsername(string _username, Action<string> successCallback, Action<string> failureCallback)
        {
            data.username = _username;
            if (successCallback != null)
                successCallback("Success");
        }

        public int CharacterCount
        {
            get { return data.characters.Count; }
        }

        public void DeleteCharacter(int slot, Action<string> successCallback, Action<string> failureCallback)
        {
            data.characters.RemoveAt(slot);
            if (successCallback != null)
                successCallback("Success");
        }

        public void DefaultSettings(Action<string> successCallback, Action<string> failureCallback)
        {
            data.userSettings = new UserSaveDataStructure.UserSettings();
            if (successCallback != null)
                successCallback("Success");
        }

        public void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<string> successCallback, Action<string> failureCallback)
        {
            data.userSettings = settings;
            if (successCallback != null)
                successCallback("Success");
        }

        public UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }

        public void Login(string username, string password, Action<string> successCallback, Action<string> failureCallback)
        {
            CreateGameObjectSaveDataHandler();
            SetUsername(username, successCallback, failureCallback);
        }
    }
}