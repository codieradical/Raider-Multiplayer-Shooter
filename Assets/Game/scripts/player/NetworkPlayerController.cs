using Raider.Game.Gametypes;
using Raider.Game.Networking;
using Raider.Game.Saves.User;
using Raider.Game.Weapons;
using Raider.Game.Cameras;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Raider.Game.GUI.Scoreboard;

namespace Raider.Game.Player
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        public delegate void OnPlayerKilledPlayer(int idKilled, int idKilledBy);
        public delegate void OnPlayerRespawned(int idRespawned);
        public delegate void OnPlayerHealthChange(int playerID);
        public delegate void OnPlayerScored(int playerID);

        public static OnPlayerKilledPlayer onServerPlayerKilledPlayer;
        public static OnPlayerRespawned onServerPlayerRespawned;
		public static OnPlayerKilledPlayer onClientPlayerKilledPlayer;
		public static OnPlayerRespawned onClientPlayerRespawned;
        public static OnPlayerHealthChange onClientLocalPlayerHealthChange;
        public static OnPlayerHealthChange onClientPlayerHealthChange;
        public static OnPlayerHealthChange onClientPlayerHealthDead;
        public static OnPlayerHealthChange onClientPlayerHealthAlive;
        public static OnPlayerScored onServerPlayerScored;

        private PlayerData playerData;
		public PlayerData PlayerData
		{
			get
			{
                if (playerData != null)
                    return playerData;
                else return playerData = GetComponent<PlayerData>();
			}
		}


        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f && isLocalPlayer)
                CmdSwitchWeapon(scroll);
        }

        [Command]
        public void CmdSwitchWeapon(float scroll)
        {
            if (pickupObjective != null)
            {
                CmdDropObjective();
            }

            SwitchWeapon(scroll);
        }

        void SwitchWeapon(float scroll)
        {
            ToggleWeapons(true);

            //If the player is dead, don't let them switch weapon.
            if (!PlayerData.networkPlayerController.IsAlive)
                return;

            if (scroll > 0f)
            {
                switch (PlayerData.ActiveWeaponType)
                {
                    case Armory.WeaponType.Primary:
                        PlayerData.ActiveWeaponType = Armory.WeaponType.Tertiary;
                        break;
                    case Armory.WeaponType.Secondary:
                        PlayerData.ActiveWeaponType = Armory.WeaponType.Primary;
                        break;
                    case Armory.WeaponType.Tertiary:
                        PlayerData.ActiveWeaponType = Armory.WeaponType.Secondary;
                        break;
                }
            }
            else if (scroll < 0f)
            {
                switch (PlayerData.ActiveWeaponType)
                {
                    case Armory.WeaponType.Primary:
                        PlayerData.ActiveWeaponType = Armory.WeaponType.Secondary;
                        break;
                    case Armory.WeaponType.Secondary:
                        PlayerData.ActiveWeaponType = Armory.WeaponType.Tertiary;
                        break;
                    case Armory.WeaponType.Tertiary:
                        PlayerData.ActiveWeaponType = Armory.WeaponType.Primary;
                        break;
                }
            }
        }

        public void OnDestroy()
		{
			if (NetworkServer.active && GametypeController.singleton != null && !GametypeController.singleton.isGameEnding)
				GametypeController.singleton.InactivateScoreboardPlayer(PlayerData.syncData.id, PlayerData.syncData.team);
			base.OnNetworkDestroy();
		}

        void HealthSync(int value)
        {
            Health = value;

            if (isLocalPlayer && onClientLocalPlayerHealthChange != null)
                onClientLocalPlayerHealthChange(PlayerData.syncData.id);
            
            if(onClientPlayerHealthChange != null)
                onClientPlayerHealthChange(PlayerData.syncData.id);

            if(onClientPlayerHealthDead != null)
                onClientPlayerHealthDead(PlayerData.syncData.id);

            if (onClientPlayerHealthAlive != null)
                onClientPlayerHealthAlive(PlayerData.syncData.id);
        }

        [SyncVar(hook = "HealthSync")]
        private int health = MAX_HEALTH;
		public int Health
        {
            get { return health; }
            set {
				if (value > MAX_HEALTH)
					health = MAX_HEALTH;
				else
					health = value;

				if (isLocalPlayer && onClientLocalPlayerHealthChange != null)
                    onClientLocalPlayerHealthChange(PlayerData.syncData.id);

                if (onClientPlayerHealthChange != null)
                    onClientPlayerHealthChange(PlayerData.syncData.id);

                if (onClientPlayerHealthDead != null)
                    onClientPlayerHealthDead(PlayerData.syncData.id);

                if (onClientPlayerHealthAlive != null)
                    onClientPlayerHealthAlive(PlayerData.syncData.id);
            }
        }

        public const int MAX_HEALTH = 100;

		public bool IsAlive
		{
			get
			{
				if (Health > 0)
					return true;
				else
					return false;
			}
		}

		Coroutine regenCoroutine;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="damage"></param>
		/// <param name="damageDealtBy">The ID of the player who dealt damage. -1 for no player.</param>
		[Server]
		public void TakeDamage(int damage, int damageDealtBy)
		{
			if (PlayerData.syncData.id == damageDealtBy)
				return;
			//Don't let players shoot themselves. Not sure how that would happen anyway.

			if (Health <= 0)
				return;

            PlayerData shooter = NetworkGameManager.instance.GetPlayerDataById(damageDealtBy);
            GametypeController.GameOptions gameOptions = NetworkGameManager.instance.lobbySetup.syncData.gameOptions;
            //If they're already dead, stop killing them!

            if (!GametypeController.singleton.hasInitialSpawned || GametypeController.singleton.isGameEnding)
                return;

            bool friendlyFire = PlayerData.syncData.team == shooter.syncData.team;

            //If friendlyFire is disabled, don't let friends fire!
            if (gameOptions.teamsEnabled && !gameOptions.teamOptions.friendlyFire && friendlyFire)
            {
                return;
            }

			if (regenCoroutine != null)
			{
				StopCoroutine(regenCoroutine);
				regenCoroutine = null;
			}

			Health -= damage;
			if (Health < 1)
			{
				KillPlayer(damageDealtBy);

				PlayerChatManager chatManager = PlayerData.localPlayerData.PlayerChatManager;
				chatManager.CmdSendNotificationMessage(PlayerChatManager.GetFormattedUsername(damageDealtBy) + " killed " + PlayerChatManager.GetFormattedUsername(PlayerData.syncData.id), -1);

				if (damageDealtBy > -1)
				{
					Debug.Log("I was killed by " + shooter.syncData.username);

					if (onServerPlayerKilledPlayer != null)
						onServerPlayerKilledPlayer(PlayerData.syncData.id, shooter.syncData.id);
				}
			}
			else
				regenCoroutine = StartCoroutine(RechargeHealth());
		}

		public int healthRechargeCooldownSeconds = 5;
		public int healthRechargeSteps = 10;
		public int healthRechargeTime = 5;

		[Server]
		public IEnumerator RechargeHealth()
		{
			yield return new WaitForSeconds(healthRechargeCooldownSeconds);
			for (int i = 0; i < healthRechargeSteps; i++)
			{
				health += MAX_HEALTH / healthRechargeSteps;
				yield return new WaitForSeconds((float)healthRechargeTime / healthRechargeSteps);
			}
		}

		[Server]
		public void KillPlayer(int killer)
		{
            if (pickupObjective != null)
                CmdDropObjective();

			Debug.Log("This player died.");
			TargetKillPlayer(connectionToClient);
			RpcKillPlayer(killer);

			GameObject ragDoll = Instantiate(PlayerResourceReferences.instance.GetRagdollByRace(PlayerData.PlayerSyncData.Character.Race));
			ragDoll.transform.position = this.transform.position;
			ragDoll.GetComponent<PlayerRagdoll>().UpdatePlayerAppearence(PlayerData.PlayerSyncData);
			NetworkServer.Spawn(ragDoll);
			RpcSetupRagdoll(ragDoll);

			StartCoroutine(WaitAndRespawn());
		}

		[ClientRpc]
		public void RpcSetupRagdoll(GameObject ragDoll)
		{
			ragDoll.transform.position = this.transform.position;
			ragDoll.GetComponent<PlayerRagdoll>().UpdatePlayerAppearence(PlayerData.PlayerSyncData);
		}

		[TargetRpc]
		public void TargetKillPlayer(NetworkConnection conn)
		{
			//When the player dies, switch their camera to spectate.
			for (int i = 0; i < NetworkGameManager.instance.Players.Count; i++)
				if(NetworkGameManager.instance.Players[i].PlayerSyncData.id == PlayerData.localPlayerData.PlayerSyncData.id)
				{
					CameraModeController.singleton.spectatingPlayerIndex = i;
					break;
				}

			GetComponent<MovementController>().canMove = false;
			StartCoroutine(PlayerData.localPlayerData.localPlayerController.PauseNewCameraController());

			CameraModeController.singleton.SetCameraMode(CameraModeController.CameraModes.SpectatorThirdPerson);
		}

		[ClientRpc]
		public void RpcKillPlayer(int killer)
		{
            //Hide the dead player.
            PlayerData.pickupTrigger.enabled = false;
            PlayerData.characterController.enabled = false;
			PlayerData.appearenceController.HidePlayer(true);
			PlayerData.appearenceController.usernameText.text = "";
			ToggleWeapons(false);

			if (onClientPlayerKilledPlayer != null)
				onClientPlayerKilledPlayer(PlayerData.syncData.id, killer);
		}

		public IEnumerator WaitAndRespawn()
		{
			yield return new WaitForSeconds(NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds);
			RespawnPlayer();
		}

		[Server]
		public void RespawnPlayer()
		{
			Health = MAX_HEALTH;
			Debug.Log("This player respawned.");

            if (onServerPlayerRespawned != null)
                onServerPlayerRespawned(PlayerData.syncData.id);

			TargetRespawnPlayer(connectionToClient);
			RpcRespawnPlayer();
		}

		[TargetRpc]
		public void TargetRespawnPlayer(NetworkConnection conn)
		{
			//Switch back to the normal camera.
			gameObject.transform.position = NetworkGameManager.GetSpawnPoint(PlayerData.syncData.team);
			CameraModeController.singleton.SetCameraMode(Session.userSaveDataHandler.GetSettings().Perspective);

			if (!PlayerData.localPlayerData.paused)
				GetComponent<MovementController>().canMove = true;

			StartCoroutine(PlayerData.localPlayerData.localPlayerController.PauseNewCameraController());
		}

		[ClientRpc]
		public void RpcRespawnPlayer()
		{
            PlayerData.pickupTrigger.enabled = true;
            PlayerData.characterController.enabled = true;
            PlayerData.appearenceController.HidePlayer(false);
			if(PlayerData != PlayerData.localPlayerData)
				PlayerData.appearenceController.usernameText.text = PlayerData.syncData.username;
			ToggleWeapons(true);

			if (onClientPlayerRespawned != null)
				onClientPlayerRespawned(PlayerData.syncData.id);
		}

		public void ToggleWeapons(bool active)
		{
			PlayerData playerData = GetComponent<PlayerData>();

			if (active)
			{
				PlayerData.PrimaryWeaponController.gameObject.SetActive(true);
                PlayerData.SecondaryWeaponController.gameObject.SetActive(true);
                PlayerData.TertiaryWeaponController.gameObject.SetActive(true);
			}
			else
			{
                PlayerData.PrimaryWeaponController.gameObject.SetActive(false);
                PlayerData.SecondaryWeaponController.gameObject.SetActive(false);
                PlayerData.TertiaryWeaponController.gameObject.SetActive(false);
			}
		}

        #region Objective Pickup

        public PickupGametypeObjective pickupObjective;

        [Command]
        public void CmdPickupObject(GameObject objective)
        {
            PickupGametypeObjective pickupObjective = objective.GetComponent<PickupGametypeObjective>();
            if (pickupObjective.carrierId > -1)
            {
                Debug.LogWarning("Player attempted to pickup carried objective");
                return;
            }

            pickupObjective.carrierId = PlayerData.syncData.id;

            if (PlayerData.networkPlayerController.pickupObjective != null)
                PlayerData.networkPlayerController.CmdDropObjective();

            this.pickupObjective = pickupObjective;
            pickupObjective.TogglePickup(false);
            pickupObjective.transform.SetParent(PlayerData.gunPosition.gameObject.transform, false);
            pickupObjective.transform.position = PlayerData.gunPosition.gameObject.transform.position;
            pickupObjective.transform.rotation = PlayerData.gunPosition.gameObject.transform.rotation;
            PlayerData.networkPlayerController.ToggleWeapons(false);

            if (pickupObjective.respawnObjectTimer != null)
            {
                Debug.LogError("Ending Respawn Object Timer;");
                StopCoroutine(pickupObjective.respawnObjectTimer);
                pickupObjective.respawnObjectTimer = null;
            }

            RpcPickupObject(objective);
        }

        [ClientRpc]
        private void RpcPickupObject(GameObject objective)
        {
            PickupGametypeObjective pickupObjective = objective.GetComponent<PickupGametypeObjective>();

            this.pickupObjective = pickupObjective;
            pickupObjective.TogglePickup(false);
            pickupObjective.transform.SetParent(PlayerData.gunPosition.gameObject.transform, false);
            pickupObjective.transform.position = PlayerData.gunPosition.gameObject.transform.position;
            pickupObjective.transform.rotation = PlayerData.gunPosition.gameObject.transform.rotation;
            ToggleWeapons(false);
        }

        [Command]
        public void CmdDropObjective()
        {
            pickupObjective.DropObjective();
            pickupObjective.TogglePickup(true);
            pickupObjective.carrierId = -1;

            RpcDropObjective();

            if (pickupObjective.respawnObjectTimer == null)
                pickupObjective.respawnObjectTimer = StartCoroutine(pickupObjective.WaitAndRespawnObject());

            pickupObjective = null;
            ToggleWeapons(true);
        }

        [ClientRpc]
        public void RpcDropObjective()
        {
            if (pickupObjective == null)
                return;

            pickupObjective.DropObjective();
            pickupObjective.TogglePickup(true);
            pickupObjective = null;
            ToggleWeapons(true);
        }

        [Command]
        public void CmdScoreObjective()
        {
            if (pickupObjective == null)
                return;

            pickupObjective.DropObjective();
            pickupObjective.RespawnObject();
            pickupObjective.TogglePickup(true);
            pickupObjective.carrierId = -1;
            RpcScoreObjective();
            pickupObjective = null;

            if (onServerPlayerScored != null)
                onServerPlayerScored(PlayerData.syncData.id);

            PlayerChatManager chatManager = PlayerData.localPlayerData.PlayerChatManager;
            chatManager.CmdSendNotificationMessage(PlayerChatManager.GetFormattedUsername(PlayerData.syncData.id) + " Scored!", -1);

            ToggleWeapons(true);
        }

        [ClientRpc]
        public void RpcScoreObjective()
        {
            if (pickupObjective == null)
                return;

            pickupObjective.DropObjective();
            pickupObjective.RespawnObject();
            pickupObjective.TogglePickup(true);
            pickupObjective = null;

            ToggleWeapons(true);
        }

        #endregion

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
            CmdSpawnWeapon(weapon, weaponCustomization, PlayerData.localPlayerData.PlayerSyncData.id);
        }

        [Command]
        public void CmdSpawnWeapon(Armory.Weapons weapon, WeaponSettings customization, int ownerID)
        {
            GameObject newWeapon = Instantiate(Armory.GetWeaponPrefab(weapon), NetworkGameManager.instance.GetPlayerDataById(ownerID).gunPosition.transform, false);

			Armory.WeaponType weaponType = Armory.GetWeaponType(weapon);

			newWeapon.GetComponent<WeaponController>().weaponCustomization = customization;
            newWeapon.GetComponent<WeaponController>().ownerId = ownerID;
			newWeapon.GetComponent<WeaponController>().weapon = weapon;
			newWeapon.GetComponent<WeaponController>().weaponType = weaponType;
			newWeapon.name = weapon.ToString() + ownerID;

            NetworkServer.SpawnWithClientAuthority(newWeapon, connectionToClient);

			WeaponController weaponController = newWeapon.GetComponent<WeaponController>();

			if (weaponController == null || weaponType == Armory.WeaponType.Special)
				return;
			else
			{
				switch (weaponType)
				{
					case Armory.WeaponType.Primary:
						GetComponent<PlayerData>().PrimaryWeaponController = weaponController;
						break;
					case Armory.WeaponType.Secondary:
						GetComponent<PlayerData>().SecondaryWeaponController = weaponController;
						break;
					case Armory.WeaponType.Tertiary:
						GetComponent<PlayerData>().TertiaryWeaponController = weaponController;
						break;
				}
			}

			GetComponent<NetworkPlayerSetup>().TargetSpawnedWeapon(connectionToClient, newWeapon, weapon);
        }
    }
}
