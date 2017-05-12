using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Raider.Game.Player;

namespace Raider.Game.Gametypes
{
    public class GametypeObjective : NetworkBehaviour
    {
        public GametypeHelper.Team team;
        public GametypeObjectiveSpawnPoint.Objective objective;
        public GametypeHelper.Gametype gametype;
    }
}