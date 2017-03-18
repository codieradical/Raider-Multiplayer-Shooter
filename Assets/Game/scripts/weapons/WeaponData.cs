using UnityEngine;

namespace Raider.Game.Weapons
{
    //Since weapons might have special abilities, they need seperate classes.
    //But all inherit some similarities, like the crosshair.
    public abstract class WeaponData : MonoBehaviour
    {
        public enum WeaponTypes
        {
            Unarmed = 0,
            ScoutRifle = 1
        }

        public abstract string WEAPON_NAME { get; }

        //Assigned in Editor.
        public Sprite crosshair;
        public Sprite blueprints;
        public Sprite icon;
        public GameObject weaponObject;

        public int clipAmmo; //The ammo in the clip.
        public int totalAmmo; //Backpack ammo.

        public WeaponSettings slowWeapSettings;
        public WeaponSettings midWeapSettings;
        public WeaponSettings fastWeapSettings;

        public class WeaponSettings
        {
            public Armory.Weapons weaponType;
            public int range;
            public float reloadTime;
            public float damagePerShot;
            public int clipSize;
            public int maxAmmo;
            public float fireRate;
            public float bulletSpread;
        }

        //Three tiers should be available. Slow and powerful, mid, fast and weak.
        public WeaponSettings chosenSettings;

        public void SpawnWeapon()
        {
            //Instantiate(weaponObject)
        }
    }
}