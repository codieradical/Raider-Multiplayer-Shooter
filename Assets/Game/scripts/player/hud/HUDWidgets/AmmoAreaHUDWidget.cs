using UnityEngine;
using UnityEngine.UI;
using Raider.Game.Player;
using Raider.Game.Weapons;

public class AmmoAreaHUDWidget : MonoBehaviour
{
    public Text loadedAmmo;
    public Text backpackAmmo;
    public Image weaponSchematic;
    
    public void UpdateWidgetData()
    {
        WeaponController weaponController = PlayerData.localPlayerData.ActiveWeaponController;
        loadedAmmo.text = weaponController.clipAmmo.ToString();
        backpackAmmo.text = weaponController.totalAmmo.ToString();
        weaponSchematic.sprite = weaponController.gameObject.GetComponent<WeaponHUDData>().blueprints;
    }
}
