﻿using Raider.Game.Cameras;
using System.Collections;
using UnityEngine;

namespace Raider.Game.Player
{
	[RequireComponent(typeof(PlayerData))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(AnimationParametersController))]
    [RequireComponent(typeof(PlayerResourceReferences))]
    public class PlayerSetup : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            PlayerData playerData = GetComponent<PlayerData>();
            PlayerData.localPlayerData = playerData;

            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");

            SetupLocalPlayer();

            PlayerData.localPlayerData.PlayerSyncData.Character = Session.ActiveCharacter;
            PlayerData.localPlayerData.PlayerSyncData.username = Session.userSaveDataHandler.GetUsername();
            PlayerData.localPlayerData.PlayerSyncData.isLeader = true;

            PlayerData.localPlayerData.localPlayerController.UpdatePerspective(Session.userSaveDataHandler.GetSettings().Perspective);
        }

        IEnumerator PauseNewCameraController()
        {
            yield return 0;
            yield return 0;
            if(PlayerData.localPlayerData.paused)
                CameraModeController.singleton.GetCameraController().enabled = false;
        }

        void SetupLocalPlayer()
		{
			gameObject.name = Session.userSaveDataHandler.GetUsername();
			CameraModeController.singleton.localPlayerGameObject = gameObject;
            CameraModeController.singleton.SetCameraMode(Session.userSaveDataHandler.GetSettings().Perspective);
        }

        void OnDestroy()
        {
            //If the player is being destroyed, save the camera!
            CameraModeController.singleton.CameraParent = null;
            DontDestroyOnLoad(CameraModeController.singleton.camPoint);
            CameraModeController.singleton.enabled = true;
        }
    }
}