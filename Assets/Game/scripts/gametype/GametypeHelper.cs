using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{

	public class GametypeHelper : MonoBehaviour
	{
		#region Singleton Setup

		public static GametypeHelper instance;

		public void Awake()
		{
			DontDestroyOnLoad(this);
			if (instance != null)
				Debug.LogAssertion("It seems that multiple Gametypes are active, breaking the singleton instance.");
			instance = this;
		}

		public void OnDestroy()
		{
			Debug.Log("Gametypes destroyed, this shouldn't happen.");
			instance = null;
		}

		#endregion

		public enum Gametype
		{
			//1 represents a game scene, 2 represents a lobby scene.
			None,
			Slayer,
			Capture_The_Flag,
			King_Of_The_Hill,
			Assault,
			Oddball,
			Ui,
			Test
		}

		public enum EngineMode
		{
			None,
			Multi,
			Single,
			Ui,
			Forge
		}

		public List<GametypeData> gametypes = new List<GametypeData>();

		[Serializable]
		public class GametypeData
		{
			public string title; //Must be unique.
			public Gametype gametype;
			public GameObject controllerPrefab;
			public EngineMode engineMode;
            public List<GametypeObjectData> objects = new List<GametypeObjectData>();

            [Serializable]
            public class GametypeObjectData
            {
                public GametypeObjectiveSpawnPoint.Objective objective;
                public GameObject prefab;
            }
		}

        public GameObject GetGametypeObjectivePrefab(Gametype gametype, GametypeObjectiveSpawnPoint.Objective objective)
        {
            foreach(GametypeData gametypeData in gametypes)
            {
                if(gametypeData.gametype == gametype)
                {
                    foreach(GametypeData.GametypeObjectData objectData in gametypeData.objects)
                    {
                        if (objectData.objective == objective)
                            return objectData.prefab;
                    }
                }
            }

            Debug.LogError("Could not find gametype object prefab for " + gametype.ToString() + " " + objective.ToString());
            return null;
        }

		public void CheckAllPrefabsPresent()
		{
			foreach (Gametype gametype in Enum.GetValues(typeof(Gametype)))
			{
				bool foundInPrefabs = false;
				foreach (GametypeData gametypeData in gametypes)
				{
					if (gametypeData.gametype == gametype)
					{
						if (foundInPrefabs)
							Debug.Log("Warning, gametype " + gametype.ToString() + " was found in weapon prefabs more than once.");
						else
							foundInPrefabs = true;
					}
				}
				if (!foundInPrefabs)
					Debug.LogWarning(gametype.ToString() + " was not found in gametype prefabs list.");
			}
		}

		private void RegisterSpawnablePrefabs()
		{
			foreach (GametypeData gametype in gametypes)
			{
				ClientScene.RegisterPrefab(gametype.controllerPrefab);

                foreach(GametypeData.GametypeObjectData objective in gametype.objects)
                {
                    ClientScene.RegisterPrefab(objective.prefab);
                }
			}
		}

		private void Start()
		{
			CheckAllPrefabsPresent();
			RegisterSpawnablePrefabs();
		}

		public string GetGametypeTitle(Gametype gametype)
		{
			foreach(GametypeData gametypeData in gametypes)
			{
				if (gametypeData.gametype == gametype)
					return gametypeData.title;
			}
			throw new Exception();
		}

		public Gametype GetGametypeByTitle(string title)
		{
			foreach (GametypeData gametypeData in gametypes)
			{
				if (gametypeData.title == title)
					return gametypeData.gametype;
			}
			throw new Exception();
		}

		public GameObject GetControllerPrefabByGametype(Gametype gametype)
		{
			foreach(GametypeData controller in gametypes)
			{
				if (controller.gametype == gametype)
					return controller.controllerPrefab;
			}

			Debug.LogError("Unable to find controller for " + gametype.ToString());
			return null;
		}

		public enum Team : byte
		{
			None = 0,
			Red = 1,
			Blue = 2,
			Green = 3,
			Orange = 4,
			Purple = 5,
			Gold = 6,
			Brown = 7,
			Pink = 8
		}

		public static Color GetTeamColor(Team team)
		{
			switch (team)
			{
				case Team.Blue:
					return Color.blue;
				case Team.Brown:
					return new Color(0.54f, 0.27f, 0.07f);
				case Team.Green:
					return Color.green;
				case Team.Pink:
					return new Color(0.97f, 1f, 0.86f);
				case Team.Purple:
					return Color.magenta;
				case Team.Red:
					return Color.red;
				case Team.Gold:
					return new Color(1, 0.843f, 0);
				case Team.Orange:
					return new Color(1, 0.549f, 0);
			}

			Debug.Log("Team color out of enum!"); // Could probably default case this.
			return Color.white;
		}
	}
}
