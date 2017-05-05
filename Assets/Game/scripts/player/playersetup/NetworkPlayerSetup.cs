using Raider.Game.Cameras;
using Raider.Game.Gametypes;
using Raider.Game.GUI.Scoreboard;
using Raider.Game.Networking;
using Raider.Game.Weapons;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    [RequireComponent(typeof(PlayerChatManager))]
    [RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(PlayerResourceReferences))]
    public class NetworkPlayerSetup : NetworkBehaviour
    {
        public static NetworkPlayerSetup localPlayer;
        private PlayerData playerData;

        // Use this for initialization

        void Start()
        {
            playerData = GetComponent<PlayerData>();

            //If the player is a client, or is playing alone, add the moving mechanics.
            if (isLocalPlayer)
            {
                PlayerData.localPlayerData = playerData;

                gameObject.layer = LayerMask.NameToLayer("LocalPlayer");

				gameObject.name = Session.userSaveDataHandler.GetUsername();

                localPlayer = this;

                playerData.networkPlayerController.SpawnWeapon(playerData.PlayerSyncData.Character.PrimaryWeapon);
				playerData.networkPlayerController.SpawnWeapon(playerData.PlayerSyncData.Character.SecondaryWeapon);
				playerData.networkPlayerController.SpawnWeapon(playerData.PlayerSyncData.Character.TertiaryWeapon);
			}

            playerData.appearenceController.ReplacePlayerModel(playerData);

            if(isLocalPlayer)
            {
                playerData.appearenceController.ChangePerspectiveModel(Session.userSaveDataHandler.GetSettings().Perspective);
			}

			if(isLocalPlayer && GametypeController.singleton != null && GametypeController.singleton.hasInitialSpawned)
			{
				SetupLocalControl();
				ScoreboardHandler.UpdateHeaderMessage(NetworkGameManager.instance.lobbySetup.syncData.GametypeString);
			}

			if (isServer && GametypeController.singleton != null)
			{
				GametypeController.singleton.AddPlayerToScoreboard(playerData.PlayerSyncData.id);
			}
        }

		private void SetupLocalControl()
		{
			gameObject.AddComponent<MovementController>();
			playerData.animationController = gameObject.AddComponent<AnimationParametersController>();
			playerData.localPlayerController = gameObject.AddComponent<LocalPlayerController>();
			CameraModeController.singleton.localPlayerGameObject = gameObject;
			//CameraModeController.singleton.SetCameraMode(Session.saveDataHandler.GetSettings().perspective);
			playerData.localPlayerController.UpdatePerspective(Session.userSaveDataHandler.GetSettings().Perspective);
			ScoreboardHandler.Focus(false);
		}

        [TargetRpc]
        public void TargetSetupLocalControl(NetworkConnection conn)
        {
			SetupLocalControl();
			ScoreboardHandler.UpdateHeaderMessage(NetworkGameManager.instance.lobbySetup.syncData.GametypeString);
		}

		[TargetRpc]
		public void TargetSpawnedWeapon(NetworkConnection conn, GameObject weaponObject, Armory.Weapons weapon)
		{
			WeaponController weaponController = weaponObject.GetComponent<WeaponController>();
			Armory.WeaponType weaponType = Armory.GetWeaponType(weapon);

			if (weaponController == null || weaponType == Armory.WeaponType.Special)
				return;
			else
			{
				switch(weaponType)
				{
					case Armory.WeaponType.Primary:
						playerData.primaryWeaponController = weaponController;
						break;
					case Armory.WeaponType.Secondary:
						playerData.secondaryWeaponController = weaponController;
						break;
					case Armory.WeaponType.Tertiary:
						playerData.tertiaryWeaponController = weaponController;
						break;
				}

				if (weaponType == playerData.ActiveWeaponType)
					weaponController.activeWeapon = true;
			}
		}

        //Detatch Camera, Prototype.
        private void OnDestroy()
        {
            if (isLocalPlayer)
            {
                if (CameraModeController.singleton.GetCameraController() is PlayerCameraController)
                    CameraModeController.singleton.gameObject.transform.SetParent(null, false);
            }
        }
	}
}
