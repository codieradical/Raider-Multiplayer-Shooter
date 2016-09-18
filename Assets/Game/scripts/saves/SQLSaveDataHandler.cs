using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.Saves {

    public class SQLSaveDataHandler : ISaveDataHandler {
        public int characterCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void DeleteCharacter(int slot)
        {
            throw new NotImplementedException();
        }

        public void DeleteData()
        {
            throw new NotImplementedException();
        }

        public List<SaveDataStructure.Character> GetAllCharacters()
        {
            throw new NotImplementedException();
        }

        public SaveDataStructure.Character GetCharacter(int slot)
        {
            throw new NotImplementedException();
        }

        public string GetUsername()
        {
            throw new NotImplementedException();
        }

        public void NewCharacter(SaveDataStructure.Character character)
        {
            throw new NotImplementedException();
        }

        public void NewData()
        {
            throw new NotImplementedException();
        }

        public SaveDataStructure ReadData()
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            throw new NotImplementedException();
        }

        public void SaveCharacter(int slot, SaveDataStructure.Character character)
        {
            throw new NotImplementedException();
        }

        public void SaveData(SaveDataStructure _data)
        {
            throw new NotImplementedException();
        }

        public void SetUsername(string _username)
        {
            throw new NotImplementedException();
        }
    }
}