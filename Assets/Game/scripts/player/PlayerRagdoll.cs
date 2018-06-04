using Raider.Game.Gametypes;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Networking;
using UnityEngine;

namespace Raider.Game.Player
{
	public class PlayerRagdoll : CharacterPreviewAppearenceController
    {
        public delegate void RagdollSpawned();
        public delegate void RagdollDespawned();

        public static RagdollSpawned onRagdollSpawn;
        public static RagdollDespawned onRagdollDespawn;

        float spawnedTime;
        float ragdollDuration = 30;
        public PlayerData owner;

        // Use this for initialization
        void Start()
        {
            if (onRagdollSpawn != null)
                onRagdollSpawn();

            spawnedTime = Time.time;
        }

        public void UpdatePlayerAppearence(PlayerData owner)
        {
            this.owner = owner;
            base.UpdatePlayerAppearence(owner.syncData.Character);

            if (owner.syncData.team != GametypeHelper.Team.None)
            {
                foreach (Renderer primaryRenderer in primaryRenderers)
                {
                    primaryRenderer.material.color = GametypeHelper.GetTeamColor(owner.syncData.team);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > spawnedTime + 30 + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds)
                Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            if (onRagdollDespawn != null)
                onRagdollDespawn();
        }
    }
}