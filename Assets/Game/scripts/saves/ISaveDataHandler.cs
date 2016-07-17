using UnityEngine;
using System.Collections;

public interface ISaveDataHandler
{
    SaveDataStructure ReadData();
    void SaveCharacter(int slot, SaveDataStructure.Character character);
    SaveDataStructure.Character LoadCharacter(int slot);
    void SaveData(SaveDataStructure _data);
    void ReloadData();
}
