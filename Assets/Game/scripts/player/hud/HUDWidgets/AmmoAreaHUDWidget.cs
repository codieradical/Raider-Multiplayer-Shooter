using UnityEngine;
using UnityEngine.UI;
using Raider.Game.Player;
using Raider.Game.Weapons;

namespace Raider.Game.Player.HUD
{
    public class AmmoAreaHUDWidget : MonoBehaviour
    {
        public Text loadedAmmo;
        public Text backpackAmmo;
        public Image weaponSchematic;

        private void Update()
        {
            UpdateWidgetData();
        }

        public void UpdateWidgetData()
        {
            WeaponController weaponController = PlayerData.localPlayerData.ActiveWeaponController;
            if (weaponController != null)
            {
                loadedAmmo.text = weaponController.clipAmmo.ToString();
                backpackAmmo.text = weaponController.totalAmmo.ToString();
                weaponSchematic.sprite = weaponController.gameObject.GetComponent<WeaponHUDData>().blueprints;
            }
        }
    }
}