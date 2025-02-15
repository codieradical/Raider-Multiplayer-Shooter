﻿using Raider.Game.Cameras;
using Raider.Game.Gametypes;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.Scoreboard;
using Raider.Game.GUI.Screens;
using Raider.Game.GUI.StartMenu;
using Raider.Game.Saves.User;
using Raider.Game.Scene;
using System;
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
			OpenCloseScoreboard();
			UpdateWeaponRotation();
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

			if (PlayerData.localPlayerData.networkPlayerController.IsAlive)
				GetComponent<MovementController>().canMove = true;

			PlayerData.localPlayerData.animationController.enabled = true;
			CameraModeController.singleton.GetCameraController().enabled = true;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public IEnumerator PauseNewCameraController()
		{
			yield return 0;
			yield return 0;
			if (PlayerData.localPlayerData.paused)
				CameraModeController.singleton.GetCameraController().enabled = false;
		}

		void OpenCloseScoreboard()
		{
			if (Scenario.InLobby)
				return;

			if (Input.GetKeyDown(KeyCode.Tab))
			{

				//Don't allow it to open with other ui...
				if (StartMenuHandler.instance.IsOpen)
					return;
				if (OptionsPaneHandler.IsOpen())
					return;
				if (ChatUiHandler.instance.IsOpen)
					return;

				ScoreboardHandler.Open = true;
			}
			else if(Input.GetKeyUp(KeyCode.Tab) && ScoreboardHandler.Open == true)
			{
				if (GametypeController.singleton.isGameEnding)
					return;

				ScoreboardHandler.Open = false;
			}
		}

		void UpdateWeaponRotation()
		{
			try
			{
				if (CameraModeController.singleton.GetCameraController() is PlayerCameraController)
					PlayerData.localPlayerData.gunPosition.transform.parent.rotation = CameraModeController.singleton.cam.transform.rotation;
			}
			catch (NullReferenceException)
			{
			}
		}
    }
}