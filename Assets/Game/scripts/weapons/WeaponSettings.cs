
namespace Raider.Game.Weapons
{
    [System.Serializable]
    public class WeaponSettings
    {
        public WeaponSettings()
        {
             
        }

        public WeaponSettings(int projectileCount, int range, float reloadTime, float damagePerShot, int clipSize, int maxAmmo, float fireRate, float bulletSpread)
        {
            this.projectileCount = projectileCount;
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
        /// Recoil recieved from firing.
        /// </summary>
        public float recoil = 0.2f;
        /// <summary>
        /// The amount of projectiles fired per shot.
        /// </summary>
        public int projectileCount = 1;
        /// <summary>
        /// Bullet range, in meters.
        /// </summary>
        public int range = 100;
        /// <summary>
        /// Reload time in seconds.
        /// </summary>
        public float reloadTime = 2;
        /// <summary>
        /// Damage dealt per bullet.
        /// </summary>
        public float damagePerShot = 10;
        /// <summary>
        /// The maximum amount of ammo in a clip.
        /// </summary>
        public int clipSize = 10;
        /// <summary>
        /// The maximum amount of ammo in the mag.
        /// </summary>
        public int maxAmmo = 100;
        /// <summary>
        /// Seconds between shots.
        /// </summary>
        public float fireRate = 0.2f;
        /// <summary>
        /// The amount a bullet can spread from the crosshair in any given direction, in units.
        /// </summary>
        public float bulletSpread = 1f;
    }
}