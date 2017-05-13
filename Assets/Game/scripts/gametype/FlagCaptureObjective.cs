using UnityEngine;
using System.Collections;
using Raider.Game.Player;

namespace Raider.Game.Gametypes
{

    public class FlagCaptureObjective : GametypeObjective
    {

        protected virtual void OnTriggerStay(Collider other)
        {
            CheckFlagHeld(other);
        }

        protected virtual void CheckFlagHeld(Collider other)
        {
            PlayerData playerData = null;
            playerData = other.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            if (playerData != PlayerData.localPlayerData)
                return;

            if (playerData.syncData.team != team)
                return;

            if (playerData.networkPlayerController.pickupObjective == null)
                return;

            if (playerData.networkPlayerController.pickupObjective is FlagObjective)
            {
                if(playerData.networkPlayerController.pickupObjective.team != team)
                    playerData.networkPlayerController.CmdScoreObjective();
            }
        }
    }
}