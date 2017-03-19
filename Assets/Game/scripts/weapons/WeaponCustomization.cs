
namespace Raider.Game.Weapons
{
    [System.Serializable]
    public class WeaponCustomization
    {
        public WeaponCustomization()
        {
             
        }

        public WeaponCustomization(Armory.Weapons weaponType, int range, float reloadTime, float damagePerShot, int clipSize, int maxAmmo, float fireRate, float bulletSpread)
        {
            this.weaponType = weaponType;
            this.range = range;
            this.reloadTime = reloadTime;
            this.damagePerShot = damagePerShot;
            this.clipSize = clipSize;
            this.maxAmmo = maxAmmo;
            this.fireRate = fireRate;
            this.bulletSpread = bulletSpread;
        }

        //public int scope;
        public Armory.Weapons weaponType;
        public int range;
        public float reloadTime;
        public float damagePerShot;
        public int clipSize;
        public int maxAmmo;
        public float fireRate;
        public float bulletSpread;
    }
}