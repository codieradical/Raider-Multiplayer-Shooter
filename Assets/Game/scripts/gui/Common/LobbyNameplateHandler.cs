using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.Saves;
using Raider.Game.GUI.Layout;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{
	public class LobbyNameplateHandler : MonoBehaviour {

		public SizeOverride sizeOverride;
		public PreferredSizeOverride preferredSizeOverride;
		public EmblemHandler emblemHandler;
		public Text usernameText;
		public Text guildText;
		public Text levelText;
		public GameObject leaderIcon;
		public Image backgroundImage;
		//public GameObject speakingIcon;

		// Use this for initialization
		public void SetupNameplate(LobbyHandler.PlayerNameplate player, GameObject headerObject, GameObject parent)
		{
			sizeOverride.providedGameObject = headerObject;
			preferredSizeOverride.providedGameObject = headerObject;
			emblemHandler.UpdateEmblem (player.character);
			usernameText.text = player.username;
			guildText.text = player.character.guild;
			levelText.text = player.character.level.ToString();
			leaderIcon.SetActive (player.leader);

			float h, s, v;
			Color.RGBToHSV (player.character.armourPrimaryColor.Color, out h, out s, out v);
			Color nameplateColor = Color.HSVToRGB (h, s, 0.5f); //maybe this 0.5 is wrong...
			nameplateColor.a = 200f / 255f;
            backgroundImage.color = nameplateColor;

            transform.SetParent(parent.transform, false);
            name = player.username;
		}
	}
}