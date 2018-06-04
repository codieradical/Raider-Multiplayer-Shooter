using Raider.Game.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{
	public class OddballObjective : PickupGametypeObjective
    {

        public static NetworkPlayerController.OnPlayerAction timerScore;

        Coroutine scoreTimer;

        IEnumerator ScoreTimer()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                if (timerScore != null)
                    timerScore(carrierId);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (!NetworkServer.active)
                return;

            if (carrierId > -1 && scoreTimer == null)
                scoreTimer = StartCoroutine(ScoreTimer());
            else if (scoreTimer != null && carrierId < 0)
            {
                StopCoroutine(scoreTimer);
                scoreTimer = null;
            }
        }
    }
}