using UnityEngine;
using Raider.Game.Saves;
using System.Collections;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Cameras;
using System.Collections.Generic;
using Raider.Game.Saves.User;
using System;
using Raider.Game.Gametypes;

namespace Raider.Game.Player
{
    public class PlayerAppearenceController : CharacterPreviewAppearenceController
    {
        public GameObject firstPersonObject;

        public List<SkinnedMeshRenderer> thirdPersonRenderers;

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
            playerData.playerModel = Instantiate(PlayerResourceReferences.instance.GetModelByRace(playerData.syncData.Character.Race)) as GameObject;
            playerData.playerModel.transform.SetParent(playerData.graphicsObject.transform, false);
            playerData.playerModel.name = "Model"; //Prevents infinate (clone) appends.

            //Update the colors, emblem.
            playerData.appearenceController = playerData.playerModel.GetComponent<PlayerAppearenceController>();
            playerData.firstPersonPlayerModel = playerData.appearenceController.firstPersonObject;
            playerData.appearenceController.UpdatePlayerAppearence(playerData.syncData);

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
                {
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                emblem.gameObject.SetActive(false);
            }
            else
            {
                firstPersonObject.SetActive(false);
                foreach (SkinnedMeshRenderer meshRenderer in thirdPersonRenderers)
                {
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                emblem.gameObject.SetActive(true);
            }
        }

        public void UpdatePlayerAppearence(PlayerData.SyncData syncData)
        {
            base.UpdatePlayerAppearence(syncData.Character);

            if(syncData.team != Gametype.Teams.None)
            {
                foreach (Renderer primaryRenderer in primaryRenderers)
                {
                    primaryRenderer.material.color = Gametype.GetTeamColor(syncData.team);
                }
            }
        }
    }
}