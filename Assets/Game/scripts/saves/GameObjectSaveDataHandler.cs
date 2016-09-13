using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// The GameObject Save Data Handler is volatile! 
/// It's used for temporary storage on a script attached to a game object.
/// 
/// A plugin should be built to help with save data.
/// </summary>
public class GameObjectSaveDataHandler : MonoBehaviour, ISaveDataHandler
{
    public SaveDataStructure data;
    public GameObject parentObject;

    void Start()
    {
        parentObject = this.gameObject;
    }

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

    public int characterCount
    {
        get { return data.characters.Count; }
    }

    public void DeleteCharacter(int slot)
    {
        data.characters.RemoveAt(slot);
    }
}
