using Raider.Game.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{
	public class GametypeObjectiveSpawnPoint : NetworkBehaviour
    {
        public GametypeHelper.Gametype gametype;
        public GametypeHelper.Team team;
        public GametypeObjective.Objective objective;

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (NetworkGameManager.instance.lobbySetup.syncData.Gametype != gametype)
                Destroy(gameObject);
            else
            {
                GameObject objectiveObject = Instantiate(GametypeHelper.instance.GetGametypeObjectivePrefab(gametype, objective), transform.position, Quaternion.identity);
                GametypeObjective gametypeObjective = objectiveObject.GetComponent<GametypeObjective>();

                if(gametypeObjective is PickupGametypeObjective)
                    (gametypeObjective as PickupGametypeObjective).SetupObjective(gametype, team, objective, transform.position);
                else
                    gametypeObjective.SetupObjective(gametype, team, objective);

                NetworkServer.Spawn(objectiveObject);
            }
        }
    }
}