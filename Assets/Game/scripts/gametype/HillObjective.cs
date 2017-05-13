using UnityEngine;
using System.Collections;
using Raider.Game.Player;
using System.Collections.Generic;
using Raider.Game.Networking;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{
    public class HillObjective : GametypeObjective
    {

        public static NetworkPlayerController.OnPlayerScored timerScore;

        Coroutine scoreTimer;

        IEnumerator ScoreTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (timerScore != null)
                    timerScore(playersOnHill[0].syncData.id);
            }
        }

        List<PlayerData> playersOnHill = new List<PlayerData>();

        bool HillControlled
        {
            get
            {
                if(NetworkGameManager.instance.lobbySetup.syncData.gameOptions.teamsEnabled)
                {
                    List<GametypeHelper.Team> teamsOnHill = new List<GametypeHelper.Team>();

                    foreach(PlayerData player in playersOnHill)
                    {
                        if (teamsOnHill.Contains(player.syncData.team))
                            continue;
                        else
                            teamsOnHill.Add(player.syncData.team);
                    }

                    return teamsOnHill.Count == 1;
                }
                else
                {
                    return playersOnHill.Count == 1;
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!NetworkServer.active)
                return;

            PlayerData playerData = null;
            playerData = collider.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            playersOnHill.Add(playerData);

            if (HillControlled && scoreTimer == null)
            {
                scoreTimer = StartCoroutine(ScoreTimer());
            }
            else if (!HillControlled && scoreTimer != null)
            {
                StopCoroutine(scoreTimer);
                scoreTimer = null;
            } 
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!NetworkServer.active)
                return;

            PlayerData playerData = null;
            playerData = collider.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            playersOnHill.Remove(playerData);

            if (HillControlled && scoreTimer == null)
            {
                scoreTimer = StartCoroutine(ScoreTimer());
            }
            else if (!HillControlled && scoreTimer != null)
            {
                StopCoroutine(scoreTimer);
                scoreTimer = null;
            }
        }

    }
}
