using UnityEngine;
using System.Collections;

public interface ISaveDataHandler
{
    SaveDataStructure ReadData();
    void SaveCharacter(int slot, SaveDataStructure.Character character);
    SaveDataStructure.Character GetCharacter(int slot);
    void SaveData(SaveDataStructure _data);
    void ReloadData();
    string GetUsername();
    void SetUsername(string _username);
}
