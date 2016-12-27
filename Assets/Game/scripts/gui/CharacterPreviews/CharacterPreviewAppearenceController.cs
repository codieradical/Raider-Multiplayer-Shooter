using UnityEngine;
using System.Collections;
using Raider.Game.GUI.Components;
using Raider.Game.Saves;

namespace Raider.Game.GUI.CharacterPreviews
{
    public class CharacterPreviewAppearenceController : MonoBehaviour
    {
        //Assigned in Editor.
        public EmblemHandler emblem;
        public GameObject primaryMesh;
        public GameObject secondaryMesh;
        public GameObject tertiaryMesh;

        //Nodes used for armour in the future.
        public GameObject helmetNode;
        public GameObject shoulderNode;
        public GameObject bodyNode;

        protected Renderer primaryRenderer;
        protected Renderer secondaryRenderer;
        protected Renderer tertiaryRenderer;

        // Use this for initialization
        void Start()
        {
            if (primaryMesh == null || secondaryMesh == null /*|| tertiaryMesh == null*/)
                Debug.LogError("[Player/PlayerAppearenceController] Player appearence controller is missing a mesh.");

            primaryRenderer = primaryMesh.GetComponent<Renderer>();
            secondaryRenderer = secondaryMesh.GetComponent<Renderer>();
            //tertiaryRenderer = tertiaryMesh.GetComponent<Renderer>();
        }

        public void UpdatePlayerAppearence(SaveDataStructure.Character _character)
        {
            //If primary renderer is null, try to get it again, failing that, return.
            if (primaryRenderer == null)
            {
                Start();
                if (primaryRenderer == null)
                    return;
            }
            primaryRenderer.material.color = _character.armourPrimaryColor.Color;
            secondaryRenderer.material.color = _character.armourSecondaryColor.Color;
            //tertiaryRenderer.material.color = _character.armourTertiaryColor.color;

            emblem.UpdateEmblem(_character);
        }
    }
}