using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
using UnityEditor.Animations;
using Raider.Game.Cameras;
using Raider.Game.Saves.User;

/// <summary>
/// Anything the player needs that's assigned in inspector.
/// Mostly used for references required on client only instanced scripts.
/// </summary>
public class PlayerResourceReferences : MonoBehaviour
{
    #region Singleton Setup

    public static PlayerResourceReferences instance;

    public void Awake()
    {
        if (instance != null)
            Debug.LogAssertion("It seems that multiple Menu Managers are active, breaking the singleton instance.");
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    #endregion

    //Player Appearence Controller.
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

        public GameObject GetGraphicsByRace(UserSaveDataStructure.Character.Races race)
        {
            if (race == UserSaveDataStructure.Character.Races.X)
                return xRaceGraphics;
            else if (race == UserSaveDataStructure.Character.Races.Y)
                return yRaceGraphics;
            else
            {
                Debug.LogError("Couldn't find graphics for race " + race.ToString());
                return null;
            }
        }

        public Avatar GetAvatarByRace(UserSaveDataStructure.Character.Races race)
        {
            if (race == UserSaveDataStructure.Character.Races.X)
                return xRaceAvatar;
            else if (race == UserSaveDataStructure.Character.Races.Y)
                return yRaceAvatar;
            else
            {
                Debug.LogError("Couldn't find avatar for race " + race.ToString());
                return null;
            }
        }
    }
    [SerializeField] //For editing in the inspector.
    public RaceGraphics raceGraphics;

    //Player Animation Controller
    [System.Serializable]
    public class PlayerAnimatorControllers
    {
        public AnimatorController firstPerson;
        public AnimatorController thirdPerson;

        public AnimatorController GetControllerByPerspective(CameraModeController.CameraModes perspective)
        {
            if (perspective == CameraModeController.CameraModes.FirstPerson)
                return firstPerson;
            else
                return thirdPerson;
        }
    }
    [SerializeField]
    public PlayerAnimatorControllers animatorControllers;

}
