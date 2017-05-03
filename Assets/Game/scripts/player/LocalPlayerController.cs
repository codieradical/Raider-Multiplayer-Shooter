using Raider.Game.Cameras;
using Raider.Game.Saves.User;
using Raider.Game.Weapons;
using System.Collections;
using UnityEngine;

namespace Raider.Game.Player
{
    //Unity networking already uses PlayerController
    //And this class is specifically for players in game.
    public class LocalPlayerController : MonoBehaviour
    {
        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.CameraParent = null;
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
            CameraModeController.singleton.enabled = true;
        }

		private void Update()
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll != 0f)
				SwitchWeapon(scroll);
		}

		public void SwitchWeapon(float scroll)
		{
			//If the player is dead, don't let them switch weapon.
			if (!PlayerData.localPlayerData.networkPlayerController.IsAlive)
				return;

			if (scroll > 0f)
			{
				switch(PlayerData.localPlayerData.ActiveWeaponType)
				{
					case Armory.WeaponType.Primary:
						PlayerData.localPlayerData.ActiveWeaponType = Armory.WeaponType.Tertiary;
						break;
					case Armory.WeaponType.Secondary:
						PlayerData.localPlayerData.ActiveWeaponType = Armory.WeaponType.Primary;
						break;
					case Armory.WeaponType.Tertiary:
						PlayerData.localPlayerData.ActiveWeaponType = Armory.WeaponType.Secondary;
						break;
				}
			}
			else if (scroll < 0f)
			{
				switch (PlayerData.localPlayerData.ActiveWeaponType)
				{
					case Armory.WeaponType.Primary:
						PlayerData.localPlayerData.ActiveWeaponType = Armory.WeaponType.Secondary;
						break;
					case Armory.WeaponType.Secondary:
						PlayerData.localPlayerData.ActiveWeaponType = Armory.WeaponType.Tertiary;
						break;
					case Armory.WeaponType.Tertiary:
						PlayerData.localPlayerData.ActiveWeaponType = Armory.WeaponType.Primary;
						break;
				}
			}
		}

		public void UpdatePerspective(CameraModeController.CameraModes newPerspective)
        {
            UserSaveDataStructure.UserSettings settings = Session.userSaveDataHandler.GetSettings();
            settings.Perspective = newPerspective;
            Session.userSaveDataHandler.SaveSettings(settings, null, null);
            CameraModeController.singleton.SetCameraMode(newPerspective);

            StartCoroutine(PauseNewCameraController());

            //Replace the player model to suit the new perspective.
            PlayerData.localPlayerData.appearenceController.ChangePerspectiveModel(newPerspective);
        }

        public void PausePlayer()
        {
            PlayerData.localPlayerData.paused = true;

            GetComponent<MovementController>().canMove = false;
            PlayerData.localPlayerData.animationController.StopAnimations();
            PlayerData.localPlayerData.animationController.enabled = false;
            CameraModeController.singleton.GetCameraController().enabled = false;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void UnpausePlayer()
        {
            PlayerData.localPlayerData.paused = false;

			if(PlayerData.localPlayerData.networkPlayerController.IsAlive)
				GetComponent<MovementController>().canMove = true;

            PlayerData.localPlayerData.animationController.enabled = true;
            CameraModeController.singleton.GetCameraController().enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        IEnumerator PauseNewCameraController()
        {
            yield return 0;
            yield return 0;
            if (PlayerData.localPlayerData.paused)
                CameraModeController.singleton.GetCameraController().enabled = false;
        }
    }
}