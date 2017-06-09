using Raider.Game.GUI.Components;
using Raider.Game.Saves.User;
using System.Collections.Generic;
using UnityEngine;

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
            /*21/2/17 - If the renderers list is empty, this may be because this script has been instanced 
            and this method has been used on the same frame, before the start method has been called.
            Calling start attempts to find the renderers if a reference is not already present.*/
            if (primaryRenderers == null  || primaryRenderers.Count < 1)
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
                secondaryRenderer.material.color = _character.armourSecondaryColor.Color;
            }

            foreach (Renderer tertiaryRenderer in tertiaryRenderers)
            {
                tertiaryRenderer.material.color = _character.armourTertiaryColor.Color;
            }

            //This method is also used by first person view models which currently don't have emblems.
            if(emblem != null)
                emblem.UpdateEmblem(_character);
        }
    }
}