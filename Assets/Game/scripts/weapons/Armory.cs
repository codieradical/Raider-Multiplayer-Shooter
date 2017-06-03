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
            ScoutRifle, //BR, DMR, something like that.
            AssaultRifle, 
            //PulseRifle, //Like the famas, or destiny's pulse rifle.
            Pistol, 
            Shotgun, 
            //PlasmaRifle, //Like the beam rifle or focus rifle from Halo.
            //PlasmaLauncher, //Like destiny's fusion rifle.
            SniperRifle,
            MachineGun, 
            //Laser, //Halo style.
            //RocketLauncher,
            //GrenadeLauncher,
        }

        public enum WeaponVariation
        {
            Slow,
            Mid,
            Fast
        }

		public enum WeaponType
		{
			Primary,
			Secondary,
			Tertiary,
			Special
		}

        [Serializable]
        public class WeaponAndVariation
        {
            public WeaponAndVariation()
            {

            }

            public WeaponAndVariation(Weapons weapon, WeaponVariation variation)
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

		public static List<Weapons> GetWeaponsByType(WeaponType type)
		{
			List<Weapons> weapons = new List<Weapons>();
			foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
			{
				if (weaponPrefab.weaponType == type)
					weapons.Add(weaponPrefab.weapon);
			}
			return weapons;
		}

        public const Weapons DEFAULT_PRIMARY_WEAPON = Weapons.Pistol;
        public const Weapons DEFAULT_SECONDARY_WEAPON = Weapons.SniperRifle;
        public const Weapons DEFAULT_TERTIARY_WEAPON = Weapons.MachineGun;

        [Serializable]
        public class WeaponPrefabAndDefaults
        {
            public Weapons weapon;
			public WeaponType weaponType;
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
                    if (weaponPrefab.weapon == weapon)
                    {
                        if (foundInweaponPrefabs)
                            Debug.Log("Warning, weapon " + weapon.ToString() + " was found in weapon prefabs more than once.");
                        else
                            foundInweaponPrefabs = true;
                    }
                }
                if (!foundInweaponPrefabs)
                    Debug.LogWarning(weapon.ToString() + " was not found in weapon prefabs list.");
            }
        }

        private void RegisterSpawnablePrefabs()
        {
            foreach(WeaponPrefabAndDefaults weaponPrefab in weapons)
            {
                ClientScene.RegisterPrefab(weaponPrefab.prefab);
            }
        }

        public static GameObject GetWeaponPrefab(Weapons weapon)
        {
            foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
            {
                if (weaponPrefab.weapon == weapon)
                    return weaponPrefab.prefab;
            }
            Debug.LogError("Couldn't find prefab for weapon " + weapon.ToString());
            return null;
        }

        public static WeaponSettings GetWeaponSlowSettings(Weapons weapon)
        {
            foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
            {
                if (weaponPrefab.weapon == weapon)
                    return weaponPrefab.slowWeaponSettings;
            }
            Debug.LogError("Couldn't find slow settings for weapon " + weapon.ToString());
            return null;
        }

        public static WeaponSettings GetWeaponMidSettings(Weapons weapon)
        {
            foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
            {
                if (weaponPrefab.weapon == weapon)
                    return weaponPrefab.midWeaponSettings;
            }
            Debug.LogError("Couldn't find mid settings for weapon " + weapon.ToString());
            return null;
        }

        public static WeaponSettings GetWeaponFastSettings(Weapons weapon)
        {
            foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
            {
                if (weaponPrefab.weapon == weapon)
                    return weaponPrefab.fastWeaponSettings;
            }
            Debug.LogError("Couldn't find fast defaults for weapon " + weapon.ToString());
            return null;
        }

		public static WeaponType GetWeaponType(Weapons weapon)
		{
			foreach(WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
			{
				if (weaponPrefab.weapon == weapon)
					return weaponPrefab.weaponType;
			}
			return WeaponType.Special;
		}

        public static WeaponSettings GetWeaponSettingsByWeaponAndVariation(Weapons weapon, WeaponVariation variation)
        {
            foreach (WeaponPrefabAndDefaults weaponPrefab in instance.weapons)
            {
                if (weaponPrefab.weapon == weapon)
                {
                    switch (variation)
                    {
                        case WeaponVariation.Fast:
                            return weaponPrefab.fastWeaponSettings;
                        case WeaponVariation.Mid:
                            return weaponPrefab.midWeaponSettings;
                        case WeaponVariation.Slow:
                            return weaponPrefab.slowWeaponSettings;
                    }
                }
            }

            Debug.LogError("Couldn't find fast defaults for weapon " + weapon.ToString());
            return null;
        }

        private void Start()
        {
            CheckAllPrefabsPresent();
            RegisterSpawnablePrefabs();
        }
    }
}