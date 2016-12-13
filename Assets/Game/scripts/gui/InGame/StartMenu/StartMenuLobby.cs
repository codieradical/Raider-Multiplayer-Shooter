using UnityEngine;
using System.Collections;
using System;
using Raider.Game.Networking;

namespace Raider.Game.GUI.StartMenu
{
    public class StartMenuLobby : StartMenuPane
    {
        protected override void SetupPaneData()
        {
            NetworkManager.instance.UpdateLobbyNameplates();
        }

    }
}