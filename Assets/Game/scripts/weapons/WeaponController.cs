using UnityEngine;
using System.Collections;
using Raider.Game.Weapons;
using Raider.Game.Cameras;

namespace Raider.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        WeaponData weaponData;


        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
            else if (Input.GetKeyDown(KeyCode.Mouse0))
                Shoot();
        }


        float lastFired = 0;
        public virtual void Shoot()
        {
            if (Time.time - lastFired >= weaponData.chosenSettings.fireRate && !IsReloading)
            {
                if (weaponData.clipAmmo <= 0)
                {
                    Reload();
                    return;
                }

                Vector3 firePointPosition;
                Vector2 bulletSpread = Random.insideUnitCircle * weaponData.chosenSettings.bulletSpread;


                RaycastHit raycastHit;

#if DEBUG
                firePointPosition = CameraModeController.singleton.cam.transform.position + CameraModeController.singleton.cam.transform.forward * weaponData.chosenSettings.range;
                Debug.DrawLine(transform.position, firePointPosition, Color.red);
#endif

                firePointPosition = CameraModeController.singleton.cam.transform.position + CameraModeController.singleton.cam.transform.forward * weaponData.chosenSettings.range + CameraModeController.singleton.cam.transform.right * bulletSpread.x + CameraModeController.singleton.cam.transform.up * bulletSpread.y;

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
            get { if (Time.time - lastReload >= weaponData.chosenSettings.reloadTime) return false; else return true; }
        }
    }
}