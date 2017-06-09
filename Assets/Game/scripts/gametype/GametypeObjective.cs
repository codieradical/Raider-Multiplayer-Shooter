using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{
	public class GametypeObjective : NetworkBehaviour
    {
        public virtual void SetupObjective(GametypeHelper.Gametype gametype, GametypeHelper.Team team, Objective objective)
        {
            this.gametype = gametype;
            this.team = team;
            this.objective = objective;
        }

        public enum Objective
        {
            FlagCapture,
            Flag,
            Hill,
            Ball
        }

        [SyncVar]
        public GametypeHelper.Gametype gametype;
        [SyncVar]
        public GametypeHelper.Team team;
        [SyncVar]
        public Objective objective;
    }
}