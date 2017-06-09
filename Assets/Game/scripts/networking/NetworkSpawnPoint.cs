using Raider.Game.Gametypes;
using UnityEngine;

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
