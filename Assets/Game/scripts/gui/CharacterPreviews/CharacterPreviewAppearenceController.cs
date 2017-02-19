using UnityEngine;
using System.Collections;
using Raider.Game.GUI.Components;
using Raider.Game.Saves;
using Raider.Game.Saves.User;
using System.Collections.Generic;

namespace Raider.Game.GUI.CharacterPreviews
{
    public class CharacterPreviewAppearenceController : MonoBehaviour
    {
        //Assigned in Editor.
        public EmblemHandler emblem;

        //Also assigned in editor, so doesn't need instancing.
        public List<GameObject> primaryMeshObjects;
        public List<GameObject> secondaryMeshObjects;
        public List<GameObject> tertiaryMeshObjects;

        //Nodes used for armour in the future.
        //public GameObject helmetNode;
        //public GameObject shoulderNode;
        //public GameObject bodyNode;

        protected List<Renderer> primaryRenderers = new List<Renderer>();
        protected List<Renderer> secondaryRenderers = new List<Renderer>();
        protected List<Renderer> tertiaryRenderers = new List<Renderer>();

        // Use this for initialization
        void Start()
        {
            if (primaryMeshObjects == null || secondaryMeshObjects == null /*|| tertiaryMesh == null*/)
                Debug.LogError("[Player/PlayerAppearenceController] Player appearence controller is missing a mesh.");

            foreach(GameObject meshObject in primaryMeshObjects)
            {
                primaryRenderers.Add(meshObject.GetComponent<Renderer>());
            }

            foreach (GameObject meshObject in secondaryMeshObjects)
            {
                secondaryRenderers.Add(meshObject.GetComponent<Renderer>());
            }

            foreach (GameObject meshObject in tertiaryMeshObjects)
            {
                tertiaryRenderers.Add(meshObject.GetComponent<Renderer>());
            }
        }

        public void UpdatePlayerAppearence(UserSaveDataStructure.Character _character)
        {
            //If primary renderer is null, try to get it again, failing that, return.
            //15/2/17 - Why?
            if (primaryRenderers == null)
            {
                Start();
                if (primaryRenderers == null)
                    return;
            }

            foreach(Renderer primaryRenderer in primaryRenderers)
            {
                primaryRenderer.material.color = _character.armourPrimaryColor.Color;
            }

            foreach (Renderer secondaryRenderer in secondaryRenderers)
            {
                secondaryRenderer.material.color = _character.armourPrimaryColor.Color;
            }

            foreach (Renderer tertiaryRenderer in tertiaryRenderers)
            {
                tertiaryRenderer.material.color = _character.armourPrimaryColor.Color;
            }

            emblem.UpdateEmblem(_character);
        }
    }
}