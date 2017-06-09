using Raider.Game.Cameras;
using Raider.Game.GUI.CharacterPreviews;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.Player
{
	public class PlayerAppearenceController : CharacterPreviewAppearenceController
    {
        public GameObject firstPersonObject;
		public List<SkinnedMeshRenderer> sharedRenderers;
        public List<SkinnedMeshRenderer> thirdPersonRenderers;
        public Text usernameText;

        private void Awake()
        {
            if(PlayerResourceReferences.instance != null)
                PlayerResourceReferences.instance.CheckAllRaceModelsPresent();
        }

        /// <summary>
        /// Creates a new model, and deletes the existing instance.
        /// Returns the new PlayerAppearenceController.
        /// </summary>
        /// <param name="playerData">The appearence data for the model.</param>
        public void ReplacePlayerModel(PlayerData playerData)
        {
            //Spawn the model.
            playerData.playerModel = Instantiate(PlayerResourceReferences.instance.GetModelByRace(playerData.PlayerSyncData.Character.Race)) as GameObject;
            playerData.playerModel.transform.SetParent(playerData.graphicsObject.transform, false);
            playerData.playerModel.name = "Model"; //Prevents infinate (clone) appends.

            //Update the colors, emblem.
            playerData.appearenceController = playerData.playerModel.GetComponent<PlayerAppearenceController>();
            playerData.firstPersonPlayerModel = playerData.appearenceController.firstPersonObject;
            playerData.appearenceController.UpdatePlayerAppearence(playerData.PlayerSyncData);

            if (playerData.IsLocalPlayer) //If the local player's model is being recreated, make sure to update the perspective.
                ChangePerspectiveModel(CameraModeController.singleton.CameraMode);

            //This old appearence controller and graphics object is no longer needed.
            Destroy(gameObject);
        }

        public void ChangePerspectiveModel(CameraModeController.CameraModes perspective)
        {
            if(perspective == CameraModeController.CameraModes.FirstPerson)
            {
                firstPersonObject.SetActive(true);
                foreach (SkinnedMeshRenderer meshRenderer in thirdPersonRenderers)
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                emblem.gameObject.SetActive(false);
            }
            else
            {
                firstPersonObject.SetActive(false);
                foreach (SkinnedMeshRenderer meshRenderer in thirdPersonRenderers)
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                emblem.gameObject.SetActive(true);
            }
        }

        public void UpdatePlayerAppearence(PlayerData.SyncData syncData)
        {
            base.UpdatePlayerAppearence(syncData.Character);

            if(syncData.team != Gametypes.GametypeHelper.Team.None)
            {
                foreach (Renderer primaryRenderer in primaryRenderers)
                {
                    primaryRenderer.material.color = Gametypes.GametypeHelper.GetTeamColor(syncData.team);
                }
            }

            if (PlayerData.localPlayerData.syncData == syncData)
                usernameText.text = "";
            else
                usernameText.text = syncData.username;
        }

		public void HidePlayer(bool hide)
		{
			if (hide)
			{
				foreach (SkinnedMeshRenderer meshRenderer in thirdPersonRenderers)
					meshRenderer.enabled = false;
				foreach (SkinnedMeshRenderer sharedRenderer in sharedRenderers)
					sharedRenderer.enabled = false;

				firstPersonObject.SetActive(false);
				emblem.gameObject.SetActive(false);
			}
			else
			{
				foreach (SkinnedMeshRenderer sharedRenderer in sharedRenderers)
					sharedRenderer.enabled = true;

				if (Session.userSaveDataHandler.GetSettings().Perspective == CameraModeController.CameraModes.FirstPerson && this == PlayerData.localPlayerData.appearenceController)
				{
					firstPersonObject.SetActive(true);
					foreach (SkinnedMeshRenderer meshRenderer in thirdPersonRenderers)
					{
						meshRenderer.enabled = true;
						meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
					}
					emblem.gameObject.SetActive(false);
				}
				else
				{
					firstPersonObject.SetActive(false);
					foreach (SkinnedMeshRenderer meshRenderer in thirdPersonRenderers)
					{
						meshRenderer.enabled = true;
						meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					}
					emblem.gameObject.SetActive(true);
				}
			}
		}
    }
}