using Raider.Game.GUI.Layout;
using Raider.Game.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public void UpdateColor()
        {
            if (playerData.team == Gametypes.GametypeHelper.Team.None)
            {
                float h, s, v;
                Color.RGBToHSV(playerData.Character.armourPrimaryColor.Color, out h, out s, out v);
                Color nameplateColor = Color.HSVToRGB(h, s, v);
                nameplateColor.a = 200f / 255f;
                backgroundImage.color = nameplateColor;
            }
            else
            {
                Color nameplateColor = Gametypes.GametypeHelper.GetTeamColor(playerData.team);
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

            transform.SetParent(parent.transform, false);
            name = player.username;

            playerData = player;

            UpdateColor();
        }
	}
}