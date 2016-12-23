using UnityEngine;
using Raider.Game.Saves;
using System.Collections;
using Raider.Game.GUI.Components;

namespace Raider.Game.Player
{
    public class PlayerAppearenceController : MonoBehaviour
    {
        [System.Serializable]
        public class RaceGraphics
        {
            public GameObject xRaceGraphics;
            public GameObject yRaceGraphics;
            public Avatar xRaceAvatar;
            public Avatar yRaceAvatar;

            public void CheckAllGraphicsPresent()
            {
                if (xRaceGraphics == null || yRaceGraphics == null || xRaceAvatar == null || yRaceAvatar == null)
                    Debug.LogError("The player is missing a model prefab or avatar!!!");
            }

            public GameObject GetGraphicsByRace(SaveDataStructure.Character.Race race)
            {
                if (race == SaveDataStructure.Character.Race.X)
                    return xRaceGraphics;
                else if (race == SaveDataStructure.Character.Race.Y)
                    return yRaceGraphics;
                else
                {
                    Debug.LogError("Couldn't find graphics for race " + race.ToString());
                    return null;
                }
            }

            public Avatar GetAvatarByRace(SaveDataStructure.Character.Race race)
            {
                if (race == SaveDataStructure.Character.Race.X)
                    return xRaceAvatar;
                else if (race == SaveDataStructure.Character.Race.Y)
                    return yRaceAvatar;
                else
                {
                    Debug.LogError("Couldn't find avatar for race " + race.ToString());
                    return null;
                }
            }
        }
        [SerializeField] //For editing in the inspector.
        private RaceGraphics editorRaceGraphics;
        public static RaceGraphics GetRaceGraphics { get; private set; }

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

        private void Awake()
        {
            editorRaceGraphics.CheckAllGraphicsPresent();
            GetRaceGraphics = editorRaceGraphics;
        }

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

        public static void SetupGraphicsModel(PlayerData playerData)
        {
            playerData.animator.enabled = false;

            //Spawn the graphics.
            playerData.graphicsObject = Instantiate(GetRaceGraphics.GetGraphicsByRace(playerData.character.race)) as GameObject;
            playerData.graphicsObject.transform.SetParent(playerData.gameObject.transform, false);

            //Setup the animator
            playerData.animator.avatar = GetRaceGraphics.GetAvatarByRace(playerData.character.race);

            //Update the colors, emblem.
            playerData.graphicsObject.GetComponent<PlayerAppearenceController>().UpdatePlayerAppearence(playerData.character);

            playerData.animator.enabled = true;
        }
    }
}