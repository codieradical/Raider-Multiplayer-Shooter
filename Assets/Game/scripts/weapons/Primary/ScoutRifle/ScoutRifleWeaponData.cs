using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raider.Game.Weapons
{
    public class ScoutRifleWeaponData : WeaponData
    {
        public override string WEAPON_NAME
        {
            get
            {
                return "Scout Rifle";
            }
        }

        public void Awake()
        {
            slowWeapSettings = new WeaponSettings
            {
                weaponType = Armory.Weapons.ScoutRifle,
                range = 100,
                reloadTime = 3,
                damagePerShot = 3,
                clipSize = 14,
                maxAmmo = 180,
                fireRate = 4,
                bulletSpread = 4
            };

            midWeapSettings = new WeaponSettings {
                weaponType = Armory.Weapons.ScoutRifle,
                range = 100,
                reloadTime = 2.75f,
                damagePerShot = 2.75f,
                clipSize = 18,
                maxAmmo = 220,
                fireRate = 3,
                bulletSpread = 4.25f
            };

            fastWeapSettings = new WeaponSettings {
                weaponType = Armory.Weapons.ScoutRifle,
                range = 100,
                reloadTime = 2.2f,
                damagePerShot = 2.2f,
                clipSize = 24,
                maxAmmo = 280,
                fireRate = 2,
                bulletSpread = 4.4f
            };
        }
    }
}
