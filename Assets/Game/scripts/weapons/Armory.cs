using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Armory : MonoBehaviour
{

    #region Singleton Setup

    public static Armory instance;

    public void Awake()
    {
        if (instance != null)
            Debug.LogAssertion("It seems that multiple Armories are active, breaking the singleton instance.");
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    #endregion

    public enum Weapons
    {
        ScoutRifle
    }

    [Serializable]
    public class WeaponPrefab
    {
        public Weapons type;
        public UnityEngine.Object prefab;
    }

    public List<WeaponPrefab> weaponPrefabs;

    public void CheckAllPrefabsPresent()
    {
        foreach (Weapons weapon in Enum.GetValues(typeof(Weapons)))
        {
            bool foundInRaceModels = false;
            foreach (WeaponPrefab weaponPrefab in weaponPrefabs)
            {
                if (weaponPrefab.type == weapon)
                {
                    if (foundInRaceModels)
                        Debug.Log("Warning, weapon " + weapon.ToString() + " was found in weapon prefabs more than once.");
                    else
                        foundInRaceModels = true;
                }
            }
            if (!foundInRaceModels)
                Debug.LogError(weapon.ToString() + " was not found in weapon prefabs list.");
        }
    }

    public UnityEngine.Object GetWeaponPrefab(Weapons weaponType)
    {
        foreach (WeaponPrefab weaponPrefab in weaponPrefabs)
        {
            if (weaponPrefab.type == weaponType)
                return weaponPrefab.prefab;
        }
        Debug.LogError("Couldn't find prefab for weapon " + weaponType.ToString());
        return null;
    }

    private void Start()
    {
        CheckAllPrefabsPresent();
    }
}
