using UnityEngine;
using System.Collections;
using Raider.Game.Weapons;
using Raider.Game.Cameras;

namespace Raider.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        Weapon weapon;


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
            if (Time.time - lastFired >= weapon.chosenSettings.fireRate && !IsReloading)
            {
                if (weapon.clipAmmo <= 0)
                {
                    Reload();
                    return;
                }

                Vector3 firePoint;

                RaycastHit raycastHit;
                if (Physics.Raycast(CameraModeController.singleton.cam.transform.position, CameraModeController.singleton.cam.transform.forward, out raycastHit, weapon.chosenSettings.range))
                {
                    firePoint = raycastHit.point;
                }
                else
                {
                    firePoint = CameraModeController.singleton.cam.transform.position + CameraModeController.singleton.cam.transform.forward * weapon.chosenSettings.range;
                }

                Debug.DrawLine(transform.position, firePoint, Color.red, 100f);

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
            get { if (Time.time - lastReload >= weapon.chosenSettings.reloadTime) return false; else return true; }
        }
    }
}