using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raider.Game.Saves;
using Raider.Game.GUI.Layout;
using UnityEngine.UI;
using Raider.Game.Player;
using Raider.Game.Gametypes;

namespace Raider.Game.GUI.Components
{
	public class LobbyNameplateHandler : MonoBehaviour {

        public PlayerData.SyncData playerData;
		public SizeOverride sizeOverride;
		public PreferredSizeOverride preferredSizeOverride;
		public EmblemHandler emblemHandler;
		public Text usernameText;
		public Text guildText;
		public Text levelText;
		public GameObject leaderIcon;
		public Image backgroundImage;
		public GameObject speakingIcon;

        public static List<LobbyNameplateHandler> instances = new List<LobbyNameplateHandler>();

        public void Awake()
        {
            instances.Add(this);
        }

        public void OnDestroy()
        {
            instances.Remove(this);
        }

        public static void UpdateUsersNameplateColor(int userId)
        {
            foreach(LobbyNameplateHandler nameplate in instances)
            {
                if (nameplate.playerData.id == userId)
                    nameplate.UpdateColor();
            }
        }

        public void UpdateColor()
        {
            if (playerData.team == Gametype.Teams.None)
            {
                float h, s, v;
                Color.RGBToHSV(playerData.Character.armourPrimaryColor.Color, out h, out s, out v);
                Color nameplateColor = Color.HSVToRGB(h, s, v);
                nameplateColor.a = 200f / 255f;
                backgroundImage.color = nameplateColor;
            }
            else
            {
                Color nameplateColor = Gametype.GetTeamColor(playerData.team);
                nameplateColor.a = 200f / 255f;
                backgroundImage.color = nameplateColor;
            }

        }

        // Use this for initialization
        public void SetupNameplate(PlayerData.SyncData player, GameObject headerObject, GameObject parent)
		{
			sizeOverride.providedGameObject = headerObject;
			preferredSizeOverride.providedGameObject = headerObject;
			emblemHandler.UpdateEmblem (player.Character);
			usernameText.text = player.username;
			guildText.text = player.Character.guild;
			levelText.text = player.Character.level.ToString();
			leaderIcon.SetActive (player.isLeader);

			float h, s, v;
			Color.RGBToHSV (player.Character.armourPrimaryColor.Color, out h, out s, out v);
            //Color nameplateColor = Color.HSVToRGB (h, s, 0.5f); //maybe this 0.5 is wrong...
            Color nameplateColor = Color.HSVToRGB(h, s, v);
            nameplateColor.a = 200f / 255f;
            backgroundImage.color = nameplateColor;

            transform.SetParent(parent.transform, false);
            name = player.username;

            playerData = player;
		}
	}
}