using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Raider.Game.Saves
{

    public interface ISaveDataHandler
    {
        SaveDataStructure ReadData();
        void SaveData(SaveDataStructure _data);
        void ReloadData();
        void DeleteData();
        void NewData();

        void NewCharacter(SaveDataStructure.Character character);
        void SaveCharacter(int slot, SaveDataStructure.Character character);

		void DefaultSettings ();
		void SaveSettings (SaveDataStructure.Settings settings);
		SaveDataStructure.Settings GetSettings ();

        SaveDataStructure.Character GetCharacter(int slot);
        List<SaveDataStructure.Character> GetAllCharacters();
        string GetUsername();
        void SetUsername(string _username);
        int CharacterCount { get; }
        void DeleteCharacter(int slot);
    }
}