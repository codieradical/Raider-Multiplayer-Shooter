using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISaveDataHandler
{
    SaveDataStructure ReadData();
    void SaveData(SaveDataStructure _data);
    void ReloadData();
    void DeleteData();
    void NewData();

    void NewCharacter(SaveDataStructure.Character character);
    void SaveCharacter(int slot, SaveDataStructure.Character character);

    SaveDataStructure.Character GetCharacter(int slot);
    List<SaveDataStructure.Character> GetAllCharacters();
    string GetUsername();
    void SetUsername(string _username);
    int characterCount { get; }
    void DeleteCharacter(int slot);
}
