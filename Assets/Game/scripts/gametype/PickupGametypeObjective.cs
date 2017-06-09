using Raider.Game.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Gametypes
{
	[RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkTransform))]
    public abstract class PickupGametypeObjective : GametypeObjective
    {
        [SyncVar]
        public Vector3 spawnPosition;

        [SyncVar]
        public int carrierId = -1;
        public bool canPickup;

        public Rigidbody rigidBody;
        public MeshCollider meshColllider;
        public SphereCollider pickupTrigger;
        public NetworkTransform netTransform;

        public virtual void SetupObjective(GametypeHelper.Gametype gametype, GametypeHelper.Team team, Objective objective, Vector3 spawnPosition)
        {
            SetupObjective(gametype, team, objective);
            this.spawnPosition = spawnPosition;
        }

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            pickupTrigger = GetComponent<SphereCollider>();
            netTransform = GetComponent<NetworkTransform>();
        }

        protected virtual void Update()
        {
            if (pickedUpObjectThisFrame)
                pickedUpObjectThisFrame = false;
        }

        protected virtual void OnGUI()
        {
            if (!canPickup) return;
            UnityEngine.GUI.Label(new Rect(Screen.width - 150, 150, 150, 30), "Press E to pickup object");
        }

        public IEnumerator DisablePickupForSeconds(int seconds)
        {
            pickupTrigger.enabled = false;
            yield return new WaitForSeconds(seconds);
            pickupTrigger.enabled = true;
        }

        public virtual void DropObjective()
        {
            transform.SetParent(null, true);
        }

        public virtual void TogglePickup(bool enabled)
        {
            rigidBody.isKinematic = !enabled;
            meshColllider.enabled = enabled;
            pickupTrigger.enabled = enabled;
            netTransform.enabled = enabled;

            if(!enabled)
                canPickup = enabled;
            else if(enabled)
                DisablePickupForSeconds(1);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            EnterPickupRadius(other);
        }

        protected virtual void EnterPickupRadius(Collider other)
        {
            if (carrierId > -1) return;

            PlayerData playerData = null;
            playerData = other.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            if (playerData != PlayerData.localPlayerData)
                return;

            canPickup = true;
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            CheckPickupRequested(other);
        }

        public static bool pickedUpObjectThisFrame;
        protected virtual void CheckPickupRequested(Collider other)
        {
            if (pickedUpObjectThisFrame)
                return;

            if (carrierId > -1)
                canPickup = false;

            if (!canPickup) return;

            PlayerData playerData = null;
            playerData = other.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            if (playerData != PlayerData.localPlayerData)
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                canPickup = false;
                pickedUpObjectThisFrame = true;
                PlayerData.localPlayerData.networkPlayerController.CmdPickupObject(gameObject);//pickup the ball;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            ExitPickupRadius(other);
        }

        protected virtual void ExitPickupRadius(Collider other)
        {
            if (!canPickup) return;

            PlayerData playerData = null;
            playerData = other.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            if (playerData != PlayerData.localPlayerData)
                return;

            canPickup = false;
        }

        public Coroutine respawnObjectTimer;

        //If the object remains idle for 30 seconds, it may be out of bounds.
        //Respawn it.
        [Server]
        public virtual IEnumerator WaitAndRespawnObject()
        {
            yield return new WaitForSeconds(30);
            RespawnObject();
            RpcRespawnObject();

            if (team == GametypeHelper.Team.None)
                PlayerData.localPlayerData.PlayerChatManager.CmdSendNotificationMessage(objective.ToString() + " reset.", -1);
            else
                PlayerData.localPlayerData.PlayerChatManager.CmdSendNotificationMessage(team.ToString() + " Team " + objective.ToString() + " reset.", -1);
        }

        [ClientRpc]
        protected void RpcRespawnObject()
        {
            RespawnObject();
        }

        public virtual void RespawnObject()
        {
            DisablePickupForSeconds(1);
            transform.position = spawnPosition;
            transform.rotation = Quaternion.identity;
        }
    }
}