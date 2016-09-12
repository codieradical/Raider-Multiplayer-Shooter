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
public class GameObjectSaveDataHandler : ISaveDataHandler
{
    SaveDataStructure data;

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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
