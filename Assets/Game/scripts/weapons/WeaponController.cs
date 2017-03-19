using Raider.Game.Cameras;
using Raider.Game.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Weapons
{
    public class WeaponController : NetworkBehaviour
    {
        [SyncVar]
        public int ownerId;

        protected virtual void Start()
        {
            transform.SetParent(NetworkGameManager.instance.GetPlayerDataById(ownerId).transform, false);
        }

        public WeaponCustomization weaponCustomization;

        public int clipAmmo; //The ammo in the clip.
        public int totalAmmo; //Backpack ammo.

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
            else if (Input.GetKeyDown(KeyCode.Mouse0))
                Shoot();
        }


        float lastFired = 0;
        public virtual void Shoot()
        {
            if (Time.time - lastFired >= weaponCustomization.fireRate && !IsReloading)
            {
                if (clipAmmo <= 0)
                {
                    Reload();
                    return;
                }

                Vector3 firePointPosition;
                Vector2 bulletSpread = Random.insideUnitCircle * weaponCustomization.bulletSpread;


                RaycastHit raycastHit;

#if DEBUG
                firePointPosition = CameraModeController.singleton.cam.transform.position + CameraModeController.singleton.cam.transform.forward * weaponCustomization.range;
                Debug.DrawLine(transform.position, firePointPosition, Color.red);
#endif

                firePointPosition = CameraModeController.singleton.cam.transform.position + CameraModeController.singleton.cam.transform.forward * weaponCustomization.range + CameraModeController.singleton.cam.transform.right * bulletSpread.x + CameraModeController.singleton.cam.transform.up * bulletSpread.y;

                if (Physics.Linecast(CameraModeController.singleton.cam.transform.position, firePointPosition, out raycastHit))
                {
                    firePointPosition = raycastHit.point;
                }

#if DEBUG
                Debug.DrawLine(transform.position, firePointPosition, Color.magenta);
#endif

                lastFired = Time.time;
            }
        }

        float lastReload = 0;
        public virtual void Reload()
        {
            lastReload = Time.time;
        }

        bool IsReloading
        {
            get { if (Time.time - lastReload >= weaponCustomization.reloadTime) return false; else return true; }
        }
    }
}