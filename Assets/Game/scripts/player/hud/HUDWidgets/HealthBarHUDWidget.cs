using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Raider.Game.Player;
using System.Collections.Generic;

public class HealthBarHUDWidget : MonoBehaviour
{

    public List<Image> meters = new List<Image>();

    private void Start()
    {
        NetworkPlayerController.onClientLocalPlayerHealthChange += UpdateWidgetData;
    }

    public void UpdateWidgetData()
    {
        foreach (Image meter in meters)
        {
            meter.fillAmount = (float)PlayerData.localPlayerData.networkPlayerController.Health / NetworkPlayerController.MAX_HEALTH;
        }
    }
}
