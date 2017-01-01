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
            throw new NotImplementedException();
        }

        public void SaveData(SaveDataStructure _data)
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            throw new NotImplementedException();
        }

        public void DeleteData()
        {
            throw new NotImplementedException();
        }

        public void NewData()
        {
            throw new NotImplementedException();
        }

        public void NewCharacter(SaveDataStructure.Character character)
        {
            throw new NotImplementedException();
        }

        public void SaveCharacter(int slot, SaveDataStructure.Character character)
        {
            throw new NotImplementedException();
        }

        public SaveDataStructure.Character GetCharacter(int slot)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<SaveDataStructure.Character> GetAllCharacters()
        {
            throw new NotImplementedException();
        }

        public string GetUsername()
        {
            throw new NotImplementedException();
        }

        public void SetUsername(string _username)
        {
            throw new NotImplementedException();
        }

        public int CharacterCount
        {
            get { throw new NotImplementedException(); }
        }

        public void DeleteCharacter(int slot)
        {
            throw new NotImplementedException();
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