using UnityEngine;
using Raider.Game.Saves;
using System.Collections;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Cameras;
using System.Collections.Generic;

namespace Raider.Game.Player
{
    public class PlayerAppearenceController : CharacterPreviewAppearenceController
    {
        public List<SkinnedMeshRenderer> firstPersonRenderers;
        public List<SkinnedMeshRenderer> thirdPersonRenderers;

        private void Awake()
        {
            if(PlayerResourceReferences.instance != null)
                PlayerResourceReferences.instance.raceModels.CheckAllModelsPresent();
        }

        /// <summary>
        /// Creates a new model, and deletes the existing instance.
        /// Returns the new PlayerAppearenceController.
        /// </summary>
        /// <param name="playerData">The appearence data for the model.</param>
        public void ReplacePlayerModel(PlayerData playerData)
        {
            //Spawn the model.
            playerData.playerModel = Instantiate(PlayerResourceReferences.instance.raceModels.GetModelByRaceAndPerspective(playerData.character.Race)) as GameObject;
            playerData.playerModel.transform.SetParent(playerData.graphicsObject.transform, false);
            playerData.playerModel.name = "Model"; //Prevents infinate (clone) appends.

            //Find the first person model, store a reference.
            playerData.firstPersonPlayerModel = playerData.playerModel.transform.Find(PlayerData.firstPersonPlayerModelName).gameObject;

            //Update the colors, emblem.
            playerData.appearenceController = playerData.playerModel.GetComponent<PlayerAppearenceController>();
            playerData.appearenceController.UpdatePlayerAppearence(playerData.character);

            //The animator is done with this model, clear it up and get ready for the next one.
            playerData.playerModelAnimator.enabled = false;
            playerData.playerModelAnimator.avatar = null;

            if (playerData.IsLocalPlayer) //If the local player's model is being recreated, make sure to update the perspective.
                ChangePerspectiveModel(CameraModeController.singleton.CameraMode);

            //This old appearence controller and graphics object is no longer needed.
            Destroy(gameObject);
        }

        public void ChangePerspectiveModel(CameraModeController.CameraModes perspective)
        {
            if(perspective == CameraModeController.CameraModes.FirstPerson)
            {
                foreach(SkinnedMeshRenderer renderer in firstPersonRenderers)
                {
                    renderer.enabled = true;
                }
                foreach (SkinnedMeshRenderer renderer in thirdPersonRenderers)
                {
                    renderer.enabled = false;
                }
                emblem.gameObject.SetActive(false);
            }
            else
            {
                foreach (SkinnedMeshRenderer renderer in firstPersonRenderers)
                {
                    renderer.enabled = false;
                }
                foreach (SkinnedMeshRenderer renderer in thirdPersonRenderers)
                {
                    renderer.enabled = true;
                }
                emblem.gameObject.SetActive(true);
            }
        }
    }
}