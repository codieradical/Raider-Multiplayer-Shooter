using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Raider.Game.Saves
{

    /// <summary>
    /// The GameObject Save Data Handler is volatile! 
    /// It's used for temporary storage on a script attached to a game object.
    /// 
    /// A plugin should be built to help with save data.
    /// </summary>
    public class GameObjectSaveDataHandler : MonoBehaviour, ISaveDataHandler
    {
        #region Singleton Setup

        //I think I can refactor this,
        //because MonoBehaviours can't be constructed,
        //I've had to make a kinda fake constructor method.
        //Maybe an instanciatable inheriting class would work.
        public static GameObjectSaveDataHandler CreateGameObjectSaveDataHandler()
        {
            GameObject hostObject = new GameObject();
            hostObject.AddComponent<GameObjectSaveDataHandler>();
            instance.data = new SaveDataStructure();
            return instance;
        }

        public static GameObjectSaveDataHandler instance;

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

        public SaveDataStructure data;
        public GameObject parentObject;

        public SaveDataStructure ReadData()
        {
            return data;
        }

        public void SaveData(SaveDataStructure _data)
        {
            //Game object data is non-persistant.
        }

        public void ReloadData()
        {
            //Game object data is non-persistant.
        }

        public void DeleteData()
        {
            data = null;
        }

        public void NewData()
        {
            data = new SaveDataStructure();
        }

        public void NewCharacter(SaveDataStructure.Character character)
        {
            data.characters.Add(character);
        }

        public void SaveCharacter(int slot, SaveDataStructure.Character character)
        {
            data.characters[slot] = character;
        }

        public SaveDataStructure.Character GetCharacter(int slot)
        {
            return data.characters[slot];
        }

        public List<SaveDataStructure.Character> GetAllCharacters()
        {
            return data.characters;
        }

        public string GetUsername()
        {
            return data.username;
        }

        public void SetUsername(string _username)
        {
            data.username = _username;
        }

        public int CharacterCount
        {
            get { return data.characters.Count; }
        }

        public void DeleteCharacter(int slot)
        {
            data.characters.RemoveAt(slot);
        }

        public void DefaultSettings()
        {
            throw new NotImplementedException();
        }

        public void SaveSettings(SaveDataStructure.Settings settings)
        {
            throw new NotImplementedException();
        }

        public SaveDataStructure.Settings GetSettings()
        {
            throw new NotImplementedException();
        }
    }
}