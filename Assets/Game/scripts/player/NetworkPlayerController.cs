using Raider.Game.Gametypes;
using Raider.Game.Networking;
using Raider.Game.Saves.User;
using Raider.Game.Weapons;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    public class NetworkPlayerController : NetworkBehaviour
    {
		[SyncVar]
		public int health = 100;
		public bool IsAlive
		{
			get
			{
				if (health > 0)
					return true;
				else
					return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="damage"></param>
		/// <param name="damageDealtBy">The ID of the player who dealt damage. -1 for no player.</param>
		[Server]
		public void TakeDamage(int damage, int damageDealtBy)
		{
			health -= damage;
			if (health < 1)
			{ 
				KillPlayer();

				if(damageDealtBy > -1)
				{
					PlayerData player = NetworkGameManager.instance.GetPlayerDataById(damageDealtBy);

					Debug.Log("I was killed by " + player.name);
					GametypeController.singleton.AddToScoreboard(player.syncData.id, player.syncData.team, 1);
					
				}
			}
		}

		[Server]
		public void KillPlayer()
		{
			Debug.Log("This player died.");
			StartCoroutine(WaitAndRespawn());
		}

		public IEnumerator WaitAndRespawn()
		{
			yield return new WaitForSeconds(NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds);
			RespawnPlayer();
		}

		[Server]
		public void RespawnPlayer()
		{
			health = 100;
			Debug.Log("This player respawned.");
		}

        public void SpawnWeapon(Armory.Weapons weapon)
        {
            if(!isLocalPlayer)
            {
                Debug.LogWarning("Attempted to spawn weapon on non local!");
                return;
            }

            WeaponSettings weaponCustomization = Armory.GetWeaponSettingsByWeaponAndVariation(weapon, Session.ActiveCharacter.GetWeaponChosenVariation(weapon));
            if (weaponCustomization == null)
            {
                weaponCustomization = Armory.GetWeaponMidSettings(weapon);
                UserSaveDataStructure.Character character = Session.ActiveCharacter;
                character.weaponCustomizations.Add(new Armory.WeaponAndVariation(weapon, Armory.WeaponVariation.Mid));
                Session.UpdateActiveCharacter(character);
            }
            CmdSpawnWeapon(weapon, weaponCustomization, PlayerData.localPlayerData.syncData.id);
        }

        [Command]
        public void CmdSpawnWeapon(Armory.Weapons weapon, WeaponSettings customization, int ownerID)
        {
            GameObject newWeapon = Instantiate(Armory.GetWeaponPrefab(weapon));

            newWeapon.GetComponent<WeaponController>().weaponCustomization = customization;
            newWeapon.GetComponent<WeaponController>().ownerId = ownerID;

            NetworkServer.SpawnWithClientAuthority(newWeapon, connectionToClient);
        }
    }
}
