using System.Collections;
using System.Collections.Generic;
using System;
using Raider.Game.Saves;
using UnityEngine;
using UnityEngine.UI;
using Raider.Game.GUI.Layout;
using Raider.Game.GUI.Components;

public class Lobby32Demo : MonoBehaviour {

	public GameObject leftPlayersBox;
	public GameObject rightPlayersBox;
	public GameObject nameplatePrefab;
	public GameObject headerObject;

	// Use this for initialization
	void Start () {
		AddRandomPlayer ("Alex231", leftPlayersBox, true, "Anything Crafting");
		AddRandomPlayer ("Diamond_Guard", leftPlayersBox, false, "Anything Crafting");
		AddRandomPlayer ("minipasila", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Polish Viking", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("ItsGamerGR", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("beyond_mining", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Bitheral", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("J3R3", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Chauncey", leftPlayersBox, false, "ElDorito");
		AddRandomPlayer ("VoxelBlox", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("ZeroBreakdowns", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("sonicandmario85", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("sontailsfan7", leftPlayersBox, false, "Puucraft");
		AddRandomPlayer ("dirkstriderbutt", leftPlayersBox, false, "SBURB");
		AddRandomPlayer ("hussie", leftPlayersBox, false, "SBURB");
		AddRandomPlayer ("Dale Gribble", leftPlayersBox, false, "King of the Hill");

		AddRandomPlayer ("Aussiebucket", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("ASDWEW", rightPlayersBox, false, "Puucraft");
		//AddRandomPlayer ("Appletastic", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("catclient", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Edtoast_46", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("ItzCosminRO", rightPlayersBox, false, "Puucraft");
		//AddRandomPlayer ("hammerhead2090", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("isaac020410", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Leeroy Jenkins", rightPlayersBox, false, "Puucraft");
		//AddRandomPlayer ("NoName123", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("MCHopper123", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Harvester48", rightPlayersBox, false, "Puucraft");
		AddRandomPlayer ("Clef", rightPlayersBox, false, "ElDorito");
		AddRandomPlayer ("NoShots", rightPlayersBox, false, "ElDorito");
		AddRandomPlayer ("AdminIsAfk", rightPlayersBox, false, "Halo Vault");
		AddRandomPlayer ("flexxi", rightPlayersBox, false, "Halo Vault");

	}

	public void AddRandomPlayer(string username, GameObject parent, bool leader, string guild)
	{
		if (nameplatePrefab == null) {
			Debug.Log ("A lobby handler is missing a nameplate prefab.");
			Debug.LogAssertion ("Please add the prefab to any lobby in the scene.");
			throw new MissingFieldException ();
		}


		Raider.Game.Saves.SaveDataStructure.Character nameplateCharacter = new SaveDataStructure.Character ();
		nameplateCharacter.armourPrimaryColor = new Raider.Game.Saves.SaveDataStructure.SerializableColor (UnityEngine.Random.ColorHSV ());

		nameplateCharacter.emblem.layer0 = UnityEngine.Random.Range (0, EmblemHandler.layer0sprites.Length - 1);
		nameplateCharacter.emblem.layer1 = UnityEngine.Random.Range (0, EmblemHandler.layer1sprites.Length - 1);
		System.Random rand = new System.Random ();
		nameplateCharacter.emblem.layer2 = Convert.ToBoolean (rand.Next (0, 2));
		nameplateCharacter.emblem.layer0Color = new Raider.Game.Saves.SaveDataStructure.SerializableColor (UnityEngine.Random.ColorHSV ());
		nameplateCharacter.emblem.layer1Color = new Raider.Game.Saves.SaveDataStructure.SerializableColor (UnityEngine.Random.ColorHSV ());
		nameplateCharacter.emblem.layer2Color = new Raider.Game.Saves.SaveDataStructure.SerializableColor (UnityEngine.Random.ColorHSV ());

		GameObject newPlayer = Instantiate (nameplatePrefab);

		newPlayer.GetComponent<PreferredSizeOverride> ().providedGameObject = headerObject;
		newPlayer.GetComponent<SizeOverride> ().providedGameObject = headerObject;

		newPlayer.name = username;
		newPlayer.transform.FindChild ("emblem").GetComponent<EmblemHandler> ().UpdateEmblem (nameplateCharacter);

		newPlayer.transform.SetParent (parent.transform, false);
		newPlayer.transform.FindChild ("name").GetComponent<Text> ().text = username;
		newPlayer.transform.FindChild ("guild").GetComponent<Text> ().text = guild;
		newPlayer.transform.FindChild ("level").GetComponent<Text> ().text = UnityEngine.Random.Range (0, 55).ToString ();
		newPlayer.transform.FindChild ("icons").FindChild ("leader").gameObject.SetActive (leader);

		Color plateColor = nameplateCharacter.armourPrimaryColor.Color;

		float _h, _s, _v;
		Color.RGBToHSV (plateColor, out _h, out _s, out _v);

		plateColor = Color.HSVToRGB (_h, _s, 0.5f);
		plateColor.a = 200f / 255f;

		newPlayer.GetComponent<Image> ().color = plateColor;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
