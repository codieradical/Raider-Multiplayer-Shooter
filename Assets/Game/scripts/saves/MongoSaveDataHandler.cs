using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace Raider.Game.Saves
{

    public class MongoSaveDataHandler : ISaveDataHandler
    {
        public SaveDataStructure ReadData()
        {
            throw new System.NotImplementedException();
        }

        public void SaveData(SaveDataStructure _data)
        {
            throw new System.NotImplementedException();
        }

        public void ReloadData()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteData()
        {
            throw new System.NotImplementedException();
        }

        public void NewData()
        {
            throw new System.NotImplementedException();
        }

        public void NewCharacter(SaveDataStructure.Character character)
        {
            throw new System.NotImplementedException();
        }

        public void SaveCharacter(int slot, SaveDataStructure.Character character)
        {
            throw new System.NotImplementedException();
        }

        public SaveDataStructure.Character GetCharacter(int slot)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<SaveDataStructure.Character> GetAllCharacters()
        {
            throw new System.NotImplementedException();
        }

        public string GetUsername()
        {
            throw new System.NotImplementedException();
        }

        public void SetUsername(string _username)
        {
            throw new System.NotImplementedException();
        }

        public int CharacterCount
        {
            get { throw new System.NotImplementedException(); }
        }

        public void DeleteCharacter(int slot)
        {
            throw new System.NotImplementedException();
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