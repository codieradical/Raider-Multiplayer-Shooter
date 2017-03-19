
namespace Raider.Game.Weapons
{
    [System.Serializable]
    public class WeaponSettings
    {
        public WeaponSettings()
        {
             
        }

        public WeaponSettings(int range, float reloadTime, float damagePerShot, int clipSize, int maxAmmo, float fireRate, float bulletSpread)
        {
            this.range = range;
            this.reloadTime = reloadTime;
            this.damagePerShot = damagePerShot;
            this.clipSize = clipSize;
            this.maxAmmo = maxAmmo;
            this.fireRate = fireRate;
            this.bulletSpread = bulletSpread;
        }

        //public int scope;
        /// <summary>
        /// Bullet range, in meters.
        /// </summary>
        public int range;
        /// <summary>
        /// Reload time in seconds.
        /// </summary>
        public float reloadTime;
        /// <summary>
        /// Damage dealt per bullet.
        /// </summary>
        public float damagePerShot;
        /// <summary>
        /// The maximum amount of ammo in a clip.
        /// </summary>
        public int clipSize;
        /// <summary>
        /// The maximum amount of ammo in the mag.
        /// </summary>
        public int maxAmmo;
        /// <summary>
        /// Seconds between shots.
        /// </summary>
        public float fireRate;
        /// <summary>
        /// The amount a bullet can spread from the crosshair in any given direction, in units.
        /// </summary>
        public float bulletSpread;
    }
}