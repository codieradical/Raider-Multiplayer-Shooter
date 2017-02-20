using UnityEngine;
using System.Collections;
using Raider.Game.Saves;
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
    public class RaceModels
    {
        public GameObject xRaceModel;
        public GameObject yRaceModel;
        public Avatar xRaceAvatar;
        public Avatar yRaceAvatar;

        public void CheckAllModelsPresent()
        {
            if (xRaceModel == null || yRaceModel == null || xRaceAvatar == null || yRaceAvatar == null)
                Debug.LogError("The player is missing a model prefab or avatar!!!");
        }

        public GameObject GetModelByRaceAndPerspective(UserSaveDataStructure.Character.Races race)
        {
            if (race == UserSaveDataStructure.Character.Races.X)
                    return xRaceModel;
            else if (race == UserSaveDataStructure.Character.Races.Y)
                    return yRaceModel;
            else
            {
                Debug.LogError("Couldn't find model for race " + race.ToString());
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
    public RaceModels raceModels;
}
