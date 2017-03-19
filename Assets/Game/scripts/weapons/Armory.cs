using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Weapons
{

    public class Armory : MonoBehaviour
    {
        #region Singleton Setup

        public static Armory instance;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            if (instance != null)
                Debug.LogAssertion("It seems that multiple Armories are active, breaking the singleton instance.");
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        #endregion

        //1 = primary
        //2 = secondary
        //3 = tertiary
        public enum Weapons
        {
            ScoutRifle = 1,
            SniperRifle = 2,
            MachineGun = 3
        }

        public static List<Weapons> GetPrimaryWeapons()
        {
            List<Weapons> primaryWeapons = new List<Weapons>();
            foreach(Weapons weapon in Enum.GetValues(typeof(Weapons)))
            {
                if ((int)weapon == 1)
                    primaryWeapons.Add(weapon);
            }
            return primaryWeapons;
        }

        public static List<Weapons> GetSecondaryWeapons()
        {
            List<Weapons> secondaryWeapons = new List<Weapons>();
            foreach (Weapons weapon in Enum.GetValues(typeof(Weapons)))
            {
                if ((int)weapon == 2)
                    secondaryWeapons.Add(weapon);
            }
            return secondaryWeapons;
        }

        public static List<Weapons> GetTertiaryWeapons()
        {
            List<Weapons> tertiaryWeapons = new List<Weapons>();
            foreach (Weapons weapon in Enum.GetValues(typeof(Weapons)))
            {
                if ((int)weapon == 2)
                    tertiaryWeapons.Add(weapon);
            }
            return tertiaryWeapons;
        }

        public const Weapons DEFAULT_PRIMARY_WEAPON = Weapons.ScoutRifle;
        public const Weapons DEFAULT_SECONDARY_WEAPON = Weapons.SniperRifle;
        public const Weapons DEFAULT_TERTIARY_WEAPON = Weapons.MachineGun;

        [Serializable]
        public class WeaponPrefabAndDefaults
        {
            public Weapons type;
            public GameObject prefab;
            public WeaponCustomization slowWeaponCustomizationDefault = new WeaponCustomization();
            public WeaponCustomization midWeaponCustomizationDefault = new WeaponCustomization();
            public WeaponCustomization fastWeaponCustomizationDefault = new WeaponCustomization();
        }

        public List<WeaponPrefabAndDefaults> weapons;

        public void CheckAllPrefabsPresent()
        {
            foreach (Weapons weapon in Enum.GetValues(typeof(Weapons)))
            {
                bool foundInweaponPrefabs = false;
                foreach (WeaponPrefabAndDefaults weaponPrefab in weapons)
                {
                    if (weaponPrefab.type == weapon)
                    {
                        if (foundInweaponPrefabs)
                            Debug.Log("Warning, weapon " + weapon.ToString() + " was found in weapon prefabs more than once.");
                        else
                            foundInweaponPrefabs = true;
                    }
                }
                if (!foundInweaponPrefabs)
                    Debug.LogError(weapon.ToString() + " was not found in weapon prefabs list.");
            }
        }

        private void RegisterSpawnablePrefabs()
        {
            foreach(WeaponPrefabAndDefaults weapon in weapons)
            {
                ClientScene.RegisterPrefab(weapon.prefab);
            }
        }

        public static GameObject GetWeaponPrefab(Weapons weaponType)
        {
            foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
            {
                if (weaponPrefab.type == weaponType)
                    return weaponPrefab.prefab;
            }
            Debug.LogError("Couldn't find prefab for weapon " + weaponType.ToString());
            return null;
        }

        public static WeaponCustomization GetWeaponMidDefaultCustmization(Weapons weaponType)
        {
            foreach (WeaponPrefabAndDefaults weapon in instance.weapons)
            {
                if (weapon.type == weaponType)
                    return weapon.midWeaponCustomizationDefault;
            }
            Debug.LogError("Couldn't find mid defaults for weapon " + weaponType.ToString());
            return null;
        }

        private void Start()
        {
            CheckAllPrefabsPresent();
            RegisterSpawnablePrefabs();
        }
    }
}