using UnityEngine;
using System.Collections;
using Raider.Game.GUI.CharacterPreviews;
using Raider.Game.Networking;

public class PlayerRagdoll : CharacterPreviewAppearenceController
{

	float spawnedTime;
	float ragdollDuration = 30;

	// Use this for initialization
	void Start()
	{
		spawnedTime = Time.time;
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > spawnedTime + 30 + NetworkGameManager.instance.lobbySetup.syncData.gameOptions.generalOptions.respawnTimeSeconds)
			Destroy(this.gameObject);
	}
}
