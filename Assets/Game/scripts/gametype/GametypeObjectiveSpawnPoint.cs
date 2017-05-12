using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Raider.Game.Networking;

namespace Raider.Game.Gametypes
{
    public class GametypeObjectiveSpawnPoint : NetworkBehaviour
    {

        public enum Objective
        {
            FlagCapture,
            FlagSpawn,
            Hill,
            BallSpawn
        }

        public GametypeHelper.Gametype gametype;
        public GametypeHelper.Team team;

        public Objective objective;

        public override void OnStartServer()
        {
            if (NetworkGameManager.instance.lobbySetup.syncData.Gametype != gametype)
                Destroy(gameObject);
            else
                NetworkServer.Spawn(Instantiate(GametypeHelper.instance.GetGametypeObjectivePrefab(gametype, objective), transform.position, Quaternion.identity));

            base.OnStartServer();
        }
    }
}