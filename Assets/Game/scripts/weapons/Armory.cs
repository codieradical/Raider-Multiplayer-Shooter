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
            ScoutRifle = 1, //BR, DMR, something like that.
            AssaultRifle = 1, 
            PulseRifle = 1, //Like the famas, or destiny's pulse rifle.
            Pistol = 1, 
            Shotgun = 2, 
            PlasmaRifle = 2, //Like the beam rifle or focus rifle from Halo.
            PlasmaLauncher = 2, //Like destiny's fusion rifle.
            SniperRifle = 2,
            MachineGun = 3, 
            Laser = 3, //Halo style.
            RocketLauncher = 3,
            GrenadeLauncher = 3
        }

        public enum WeaponVariation
        {
            Slow = 1,
            Mid = 2,
            Fast = 3
        }

        [Serializable]
        public class WeaponTypeAndVariation
        {
            public WeaponTypeAndVariation()
            {

            }

            public WeaponTypeAndVariation(Weapons weapon, WeaponVariation variation)
            {
                Weapon = weapon;
                Variation = variation;
            }

            public string weaponString;
            public string variationString;

            public Weapons Weapon
            {
                get { return (Weapons)Enum.Parse(typeof(Weapons), weaponString); }
                set { weaponString = value.ToString(); }
            }
            public WeaponVariation Variation
            {
                get { return (WeaponVariation)Enum.Parse(typeof(WeaponVariation), variationString); }
                set { variationString = value.ToString(); }
            }
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
            public WeaponSettings slowWeaponSettings = new WeaponSettings();
            public WeaponSettings midWeaponSettings = new WeaponSettings();
            public WeaponSettings fastWeaponSettings = new WeaponSettings();
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

        public static WeaponSettings GetWeaponSlowSettings(Weapons weaponType)
        {
            foreach (WeaponPrefabAndDefaults weapon in instance.weapons)
            {
                if (weapon.type == weaponType)
                    return weapon.slowWeaponSettings;
            }
            Debug.LogError("Couldn't find slow settings for weapon " + weaponType.ToString());
            return null;
        }

        public static WeaponSettings GetWeaponMidSettings(Weapons weaponType)
        {
            foreach (WeaponPrefabAndDefaults weapon in instance.weapons)
            {
                if (weapon.type == weaponType)
                    return weapon.midWeaponSettings;
            }
            Debug.LogError("Couldn't find mid settings for weapon " + weaponType.ToString());
            return null;
        }

        public static WeaponSettings GetWeaponFastSettings(Weapons weaponType)
        {
            foreach (WeaponPrefabAndDefaults weapon in instance.weapons)
            {
                if (weapon.type == weaponType)
                    return weapon.fastWeaponSettings;
            }
            Debug.LogError("Couldn't find fast defaults for weapon " + weaponType.ToString());
            return null;
        }

        public static WeaponSettings GetWeaponSettingsByWeaponAndVariation(Weapons weaponType, WeaponVariation variation)
        {
            foreach (WeaponPrefabAndDefaults weapon in instance.weapons)
            {
                if (weapon.type == weaponType)
                {
                    switch (variation)
                    {
                        case WeaponVariation.Fast:
                            return weapon.fastWeaponSettings;
                        case WeaponVariation.Mid:
                            return weapon.midWeaponSettings;
                        case WeaponVariation.Slow:
                            return weapon.slowWeaponSettings;
                    }
                }
            }

            Debug.LogError("Couldn't find fast defaults for weapon " + weaponType.ToString());
            return null;
        }

        private void Start()
        {
            CheckAllPrefabsPresent();
            RegisterSpawnablePrefabs();
        }
    }
}