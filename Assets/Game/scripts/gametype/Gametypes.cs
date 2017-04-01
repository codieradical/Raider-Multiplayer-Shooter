using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{

	public class Gametypes : MonoBehaviour
	{
		#region Singleton Setup

		public static Gametypes instance;

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
					Debug.LogError(gametype.ToString() + " was not found in gametype prefabs list.");
			}
		}

		private void RegisterSpawnablePrefabs()
		{
			foreach (GametypeData gametype in gametypes)
			{
				ClientScene.RegisterPrefab(gametype.controllerPrefab);
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

		public enum Teams
		{
			None = 0,
			Red = 1,
			Blue = 2,
			Green = 3,
			Yellow = 4,
			Pink = 5,
			Brown = 6,
			Purple = 7,
			White = 8,
			Black = 9
		}

		public static Color GetTeamColor(Teams team)
		{
			switch (team)
			{
				case Teams.Blue:
					return Color.blue;
				case Teams.Brown:
					return new Color(0.54f, 0.27f, 0.07f);
				case Teams.Green:
					return Color.green;
				case Teams.Pink:
					return new Color(0.97f, 1f, 0.86f);
				case Teams.Purple:
					return Color.magenta;
				case Teams.Red:
					return Color.red;
				case Teams.White:
					return Color.white;
				case Teams.Yellow:
					return Color.yellow;
				case Teams.Black:
					return Color.black;
			}

			Debug.Log("Team color out of enum!"); // Could probably default case this.
			return Color.black;
		}
	}
}
