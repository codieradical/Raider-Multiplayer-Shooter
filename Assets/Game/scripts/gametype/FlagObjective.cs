using Raider.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{
	/// <summary>
	/// Handles the flag game objective.
	/// </summary>
	[RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class FlagObjective : PickupGametypeObjective
    {
        public MeshRenderer bannerRenderer;

        [SyncVar]
        public bool flagOnBase = true;
        [SyncVar]
        public bool serverReturning = false;
        public bool clientReturning = false;

		/// <summary>
		/// Used to render strings.
		/// </summary>
        protected override void OnGUI()
        {
            base.OnGUI();

            if (!clientReturning) return;
            UnityEngine.GUI.Label(new Rect(Screen.width - 150, 180, 150, 30), "Returning the flag...");
        }

        protected override void Update()
        {
            base.Update();
            if(flagOnBase && respawnObjectTimer != null)
            {
                StopCoroutine(respawnObjectTimer);
            }
        }

        public override void SetupObjective(GametypeHelper.Gametype gametype, GametypeHelper.Team team, Objective objective, Vector3 spawnPosition)
        {
            base.SetupObjective(gametype, team, objective, spawnPosition);
            UpdateFlagColor();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            UpdateFlagColor();
        }

		/// <summary>
		/// Change the flag banner color to the team color.
		/// </summary>
        void UpdateFlagColor()
        {
            bannerRenderer.material = new Material(bannerRenderer.material);
            bannerRenderer.material.color = GametypeHelper.GetTeamColor(team);
        }

        List<PlayerData> alliesOnFlag = new List<PlayerData>();
        List<PlayerData> enemiesOnFlag = new List<PlayerData>();

		/// <summary>
		/// On collision, check if a player is trying to pickup or return the flag.
		/// </summary>
		/// <param name="collider"></param>
        protected override void OnTriggerEnter(Collider collider)
        {
            EnterPickupRadius(collider);
            EnterReturnRadius(collider);
        }

		/// <summary>
		/// If the flag is on the ground and a player is attempting to return it with no enemies nearby, this method will begin returning.
		/// </summary>
		/// <param name="collider"></param>
        private void EnterReturnRadius(Collider collider)
        {
			//if someone is carrying the flag, it can't be returned.
            if (carrierId > -1) return;

            PlayerData playerData = null;
            playerData = collider.gameObject.transform.root.GetComponent<PlayerData>();

			//If there's no player on the collision, return.
            if (playerData == null)
                return;

			//If the player is on the same team as the flag, add them to the allies array.
            if (playerData.syncData.team == team)
                alliesOnFlag.Add(playerData);
            else //Else, add them to the enemies array.
                enemiesOnFlag.Add(playerData);

			//If the flag is at it's base, there's no need to return.
            if (flagOnBase) return;

			//If there's allies of the flag and no enemies, begin returning.
            if (alliesOnFlag.Count > 1 && enemiesOnFlag.Count < 1)
            {
                clientReturning = true;
                
                if (!NetworkServer.active) return;

                if (respawnObjectTimer != null)
                {
                    StopCoroutine(respawnObjectTimer);
                    respawnObjectTimer = null;
                }

                serverReturning = true;

                if (returnTimer == null)
                    returnTimer = StartCoroutine(ReturnFlagTimer());
            }
        }

        protected override void EnterPickupRadius(Collider collider)
        {
			//If the player is already being carried, allow pickup.
            if (carrierId > -1) return;

            PlayerData playerData = null;
            playerData = collider.gameObject.transform.root.GetComponent<PlayerData>();

			//If no player collided, return.
			if (playerData == null)
                return;

			//If the player that collided is not the local player (if another player is colliding), return.
			//This is a client method, so other client's shouldn't be considered.
            if (playerData != PlayerData.localPlayerData)
                return;

            if (playerData.syncData.team == team)
                return;

            canPickup = true;
        }

        protected override void OnTriggerStay(Collider other)
        {
            CheckPickupRequested(other);
        }

        protected override void OnTriggerExit(Collider collider)
        {
            base.OnTriggerExit(collider);

            ExitReturnRadius(collider);
        }

        private void ExitReturnRadius(Collider collider)
        {
            if (carrierId > -1) return;

            PlayerData playerData = null;
            playerData = collider.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            if (playerData.syncData.team == team)
                alliesOnFlag.Remove(playerData);
            else
                enemiesOnFlag.Remove(playerData);

            if (alliesOnFlag.Count < 1 || enemiesOnFlag.Count > 1)
            {
                clientReturning = false;

                if (!NetworkServer.active) return;

                if (returnTimer != null)
                {
                    StopCoroutine(returnTimer);
                    if(respawnObjectTimer == null && !flagOnBase)
                        respawnObjectTimer = StartCoroutine(WaitAndRespawnObject());
                }

                serverReturning = false;
                returnTimer = null;
            }
        }

        public override void TogglePickup(bool enabled)
        {
            base.TogglePickup(enabled);

            if (!enabled)
                flagOnBase = enabled;
        }

        Coroutine returnTimer;

        [Server]
        private IEnumerator ReturnFlagTimer()
        {
            yield return new WaitForSeconds(5);
            RespawnObject();
            RpcRespawnObject();
            flagOnBase = true;

            if (team == GametypeHelper.Team.None)
                PlayerData.localPlayerData.PlayerChatManager.CmdSendNotificationMessage(objective.ToString() + " returned.", -1);
            else
                PlayerData.localPlayerData.PlayerChatManager.CmdSendNotificationMessage(team.ToString() + " Team " + objective.ToString() + " returned.", -1);
        }

        [Server]
        public override IEnumerator WaitAndRespawnObject()
        {
            yield return base.WaitAndRespawnObject();
            flagOnBase = true;
        }

        public override void DropObjective()
        {
            base.DropObjective();

            enemiesOnFlag = new List<PlayerData>();
            alliesOnFlag = new List<PlayerData>();

            Vector3 flagRotation = transform.rotation.eulerAngles;
            flagRotation.x = 0;
            flagRotation.z = 0;
            transform.rotation = Quaternion.Euler(flagRotation);
        }

        public override void RespawnObject()
        {
            base.RespawnObject();
            enemiesOnFlag = new List<PlayerData>();
            alliesOnFlag = new List<PlayerData>();
        }
    }
}