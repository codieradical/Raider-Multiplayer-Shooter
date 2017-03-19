using Raider.Game.Networking;
using Raider.Game.Saves.User;
using Raider.Game.Weapons;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        public void SpawnWeapon(Armory.Weapons weapon)
        {
            if(!isLocalPlayer)
            {
                Debug.LogWarning("Attempted to spawn weapon on non local!");
                return;
            }

            //GameObject newWeapon = Instantiate(Armory.GetWeaponPrefab(weapon));
            //GameObject newWeapon = Armory.GetWeaponPrefab(weapon);
            WeaponCustomization weaponCustomization = Session.ActiveCharacter.GetWeaponCustomizationForWeapon(weapon);
            if (weaponCustomization == null)
            {
                weaponCustomization = Armory.GetWeaponMidDefaultCustmization(weapon);
                UserSaveDataStructure.Character character = Session.ActiveCharacter;
                character.weaponCustomizations.Add(weaponCustomization);
                Session.UpdateActiveCharacter(character);
            }
            CmdSpawnWeapon(weapon, weaponCustomization, PlayerData.localPlayerData.syncData.id);
        }

        [Command]
        public void CmdSpawnWeapon(Armory.Weapons weapon, WeaponCustomization customization, int ownerID)
        {
            GameObject newWeapon = Instantiate(Armory.GetWeaponPrefab(weapon));

            newWeapon.GetComponent<WeaponController>().weaponCustomization = customization;
            newWeapon.GetComponent<WeaponController>().ownerId = ownerID;

            NetworkServer.SpawnWithClientAuthority(newWeapon, connectionToClient);
        }
    }
}
