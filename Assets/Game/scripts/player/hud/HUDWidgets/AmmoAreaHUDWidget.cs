using UnityEngine;
using UnityEngine.UI;
using Raider.Game.Player;
using Raider.Game.Weapons;
using Raider.Game.Networking;

namespace Raider.Game.Player.HUD
{
    public class AmmoAreaHUDWidget : HUDWidget
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
				if (NetworkGameManager.instance.lobbySetup.syncData.gameOptions.infiniteAmmo)
					backpackAmmo.text = "∞";
				else
					backpackAmmo.text = weaponController.totalAmmo.ToString();
                weaponSchematic.sprite = weaponController.gameObject.GetComponent<WeaponHUDData>().blueprints;
            }
        }
    }
}