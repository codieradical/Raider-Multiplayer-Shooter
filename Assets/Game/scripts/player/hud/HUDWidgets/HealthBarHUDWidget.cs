using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Raider.Game.Player;
using System.Collections.Generic;

namespace Raider.Game.Player.HUD
{
    public class HealthBarHUDWidget : HUDWidget
	{

        public List<Image> meters = new List<Image>();

		//Something is wrong with this. Using update for now.
		//private void Start()
		//{
		//    NetworkPlayerController.onClientLocalPlayerHealthChange += UpdateWidgetData;
		//}

		private void Update()
		{
			UpdateWidgetData();
		}

		public void UpdateWidgetData()
        {
			if (PlayerData.localPlayerData == null || PlayerData.localPlayerData.networkPlayerController == null)
				return;

            foreach (Image meter in meters)
            {
                meter.fillAmount = (float)PlayerData.localPlayerData.networkPlayerController.Health / NetworkPlayerController.MAX_HEALTH;
            }
        }
    }
}