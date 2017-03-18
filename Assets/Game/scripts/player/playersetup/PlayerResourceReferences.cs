using Raider.Game.Saves.User;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Raider.Game.Player
{

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
        private class RaceAndModel
        {
            public UserSaveDataStructure.Character.Races race;
            public GameObject model;
        }

        [SerializeField]
        private List<RaceAndModel> raceModels = new List<RaceAndModel>();
        //A Tuple would be ideal, but that can't be serialized by the inspector.
        //public List<Tuple<GameObject, UserSaveDataStructure.Character.Races>> raceModels = new List<Tuple<GameObject, UserSaveDataStructure.Character.Races>>();

        public void CheckAllRaceModelsPresent()
        {
            foreach (UserSaveDataStructure.Character.Races race in Enum.GetValues(typeof(UserSaveDataStructure.Character.Races)))
            {
                bool foundInRaceModels = false;
                foreach (RaceAndModel raceModel in raceModels)
                {
                    if (raceModel.race == race)
                    {
                        if (foundInRaceModels)
                            Debug.Log("Warning, race " + race.ToString() + " was found in raceModels more than once.");
                        else
                            foundInRaceModels = true;
                    }
                }
                if (!foundInRaceModels)
                    Debug.LogError(race.ToString() + " was not found in raceModels list.");
            }
        }

        public GameObject GetModelByRace(UserSaveDataStructure.Character.Races race)
        {
            foreach (RaceAndModel raceModel in raceModels)
            {
                if (raceModel.race == race)
                    return raceModel.model;
            }
            Debug.LogError("Couldn't find model for race " + race.ToString());
            return null;
        }
    }
}