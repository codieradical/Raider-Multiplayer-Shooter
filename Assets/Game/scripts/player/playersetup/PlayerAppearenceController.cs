using UnityEngine;
using Raider.Game.Saves;
using System.Collections;
using Raider.Game.GUI.Components;
using Raider.Game.GUI.CharacterPreviews;

namespace Raider.Game.Player
{
    public class PlayerAppearenceController : CharacterPreviewAppearenceController
    {
        private void Awake()
        {
            PlayerResourceReferences.instance.raceGraphics.CheckAllGraphicsPresent();
        }

        /// <summary>
        /// Creates a new graphics model, and deletes the existing instance.
        /// Returns the new PlayerAppearenceController.
        /// </summary>
        /// <param name="playerData">The appearence data for the graphics model.</param>
        public void ReplaceGraphicsModel(PlayerData playerData)
        {
            //When switching graphics, the animator freezes.
            //Nulling the animator, then assigning after graphics setup fixes this.
            playerData.animator.avatar = null;
            playerData.animator.enabled = false;

            //Spawn the graphics.
            playerData.graphicsObject = Instantiate(PlayerResourceReferences.instance.raceGraphics.GetGraphicsByRace(playerData.character.race)) as GameObject;
            playerData.graphicsObject.transform.SetParent(playerData.gameObject.transform, false);
            playerData.graphicsObject.name = "Graphics"; //Prevents infinate (clone) appends.

            //Update the colors, emblem.
            PlayerAppearenceController newAppearenceController = playerData.graphicsObject.GetComponent<PlayerAppearenceController>();
            newAppearenceController.UpdatePlayerAppearence(playerData.character);

            //Setup the animator
            playerData.animator.enabled = true;
            playerData.animator.avatar = PlayerResourceReferences.instance.raceGraphics.GetAvatarByRace(playerData.character.race);

            //This old appearence controller and graphics object is no longer needed.
            Destroy(gameObject);
        }
    }
}