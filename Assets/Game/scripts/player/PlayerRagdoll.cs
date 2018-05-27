using Raider.Game.Gametypes;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Networking;
using UnityEngine;

namespace Raider.Game.Player
{
	public class PlayerRagdoll : CharacterPreviewAppearenceController
    {

        float spawnedTime;
        float ragdollDuration = 30;
        public GametypeHelper.Team team;

        // Use this for initialization
        void Start()
        {
            spawnedTime = Time.time;
        }

        public void UpdatePlayerAppearence(PlayerData.SyncData syncData)
        {
            base.UpdatePlayerAppearence(syncData.Character);

            if (syncData.team != GametypeHelper.Team.None)
            {
                foreach (Renderer primaryRenderer in primaryRenderers)
                {
                    primaryRenderer.material.color = GametypeHelper.GetTeamColor(syncData.team);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > spawnedTime + 30 + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds)
                Destroy(this.gameObject);
        }
    }
}