using UnityEngine;
using System.Collections;
using Raider.Game.Player;
using UnityEngine.Networking;
using Raider.Game.Networking;

namespace Raider.Game.Gametypes
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PickupGametypeObjective : GametypeObjective
    {
        [SyncVar]
        public int carrierId = -1;
        bool canPickup;

        public Rigidbody rb;
        public MeshCollider mc;
        public SphereCollider sc;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            mc = GetComponent<MeshCollider>();
            sc = GetComponent<SphereCollider>();
        }

        private void Update()
        {
            if (pickedUpObjectThisFrame)
                pickedUpObjectThisFrame = false;
        }

        private void OnGUI()
        {
            if (!canPickup) return;
            UnityEngine.GUI.Label(new Rect(Screen.width - 150, 150, 150, 30), "Press E to pickup object");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (carrierId > -1) return;


            PlayerData playerData = null;
            playerData = other.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            if (playerData.syncData.team != team)
                return;

            canPickup = true;
        }

        public static bool pickedUpObjectThisFrame;
        private void OnTriggerStay(Collider other)
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                canPickup = false;
                pickedUpObjectThisFrame = true;
                CmdPickupObject(PlayerData.localPlayerData.syncData.id);//pickup the ball;
            }
        }

        [Command]
        public void CmdPickupObject(int playerId)
        {
            if(carrierId > -1)
            {
                Debug.LogWarning("Player attempted to pickup carried objective");
                return;
            }

            carrierId = playerId;
            PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerId);

            if (playerData.networkPlayerController.pickupObjective != null)
                playerData.networkPlayerController.CmdDropObjective();

            playerData.networkPlayerController.pickupObjective = this;
            rb.isKinematic = true;
            mc.enabled = false;
            sc.enabled = false;
            transform.SetParent(playerData.gunPosition.gameObject.transform, false);
            transform.position = playerData.gunPosition.gameObject.transform.position;
            transform.rotation = playerData.gunPosition.gameObject.transform.rotation;
            playerData.networkPlayerController.ToggleWeapons(false);

            RpcPickupObject(carrierId);
        }

        [ClientRpc]
        private void RpcPickupObject(int playerId)
        {
            PlayerData playerData = NetworkGameManager.instance.GetPlayerDataById(playerId);
            playerData.networkPlayerController.pickupObjective = this;
            rb.isKinematic = true;
            mc.enabled = false;
            sc.enabled = false;
            transform.SetParent(playerData.gunPosition.gameObject.transform, false);
            transform.position = playerData.gunPosition.gameObject.transform.position;
            transform.rotation = playerData.gunPosition.gameObject.transform.rotation;
            canPickup = false;
            playerData.networkPlayerController.ToggleWeapons(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!canPickup) return;

            PlayerData playerData = null;
            playerData = other.gameObject.transform.root.GetComponent<PlayerData>();

            if (playerData == null)
                return;

            canPickup = false;
        }
    }
}