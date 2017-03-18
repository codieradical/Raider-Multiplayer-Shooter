using UnityEngine;

namespace Raider.Game.Weapons
{
    public enum WeaponTypes
    {
        Unarmed = 0,
        H3BR = 1
    }

    //Since weapons might have special abilities, they need seperate classes.
    //But all inherit some similarities, like the crosshair.
    public abstract class WeaponData : MonoBehaviour
    {
        public abstract string WEAPON_NAME { get; }

        //Assigned in Editor.
        public Sprite crosshair;
        public Sprite blueprints;
        public Sprite icon;
        public GameObject weaponObject;

        public int clipAmmo; //The ammo in the clip.
        public int totalAmmo; //Backpack ammo.

        public class WeaponSettings
        {
            public readonly int range;
            public readonly float reloadTime;
            public readonly float damagePerShot;
            public readonly int clipSize;
            public readonly int maxAmmo;
            public readonly float fireRate;
            public readonly float bulletSpread;
        }

        //Three tiers should be available. Slow and powerful, mid, fast and weak.
        public WeaponSettings chosenSettings;

        public void SpawnWeapon()
        {
            //Instantiate(weaponObject)
        }
    }
}