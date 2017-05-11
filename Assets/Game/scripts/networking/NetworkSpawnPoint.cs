using UnityEngine;
using System.Collections;
using Raider.Game.Gametypes;

namespace Raider.Game.Networking
{
	public class NetworkSpawnPoint : MonoBehaviour
	{
		public GametypeHelper.Team team;

		public void Awake()
		{
			NetworkGameManager.RegisterSpawnPoint(transform.position, team);
		}

		public void OnDestroy()
		{
			NetworkGameManager.UnregisterSpawnPoint(transform.position, team);
		}
	}
}
