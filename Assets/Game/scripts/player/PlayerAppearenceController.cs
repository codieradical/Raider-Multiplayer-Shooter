using UnityEngine;
using Raider.Game.Saves;
using System.Collections;
using Raider.Game.GUI.Components;

namespace Raider.Game.Player
{
    public class PlayerAppearenceController : MonoBehaviour
    {
        public EmblemHandler emblem;

        public GameObject primaryMesh;
        public GameObject secondaryMesh;
        public GameObject tertiaryMesh;

        //Nodes used for armour in the future.
        public GameObject helmetNode;
        public GameObject shoulderNode;
        public GameObject bodyNode;

        Renderer primaryRenderer;
        Renderer secondaryRenderer;
        Renderer tertiaryRenderer;

        // Use this for initialization
        void Start()
        {
            if (primaryMesh == null || secondaryMesh == null /*|| tertiaryMesh == null*/)
                Debug.LogError("[Player/PlayerAppearenceController] Player appearence controller is missing a mesh.");

            primaryRenderer = primaryMesh.GetComponent<Renderer>();
            secondaryRenderer = secondaryMesh.GetComponent<Renderer>();
            //tertiaryRenderer = tertiaryMesh.GetComponent<Renderer>();
        }

        public void UpdatePlayerAppearence(string _name, SaveDataStructure.Character _character)
        {
            primaryRenderer.material.color = _character.armourPrimaryColor.color;
            secondaryRenderer.material.color = _character.armourSecondaryColor.color;
            //tertiaryRenderer.material.color = _character.armourTertiaryColor.color;

            emblem.UpdateEmblem(_character);
        }
    }
}