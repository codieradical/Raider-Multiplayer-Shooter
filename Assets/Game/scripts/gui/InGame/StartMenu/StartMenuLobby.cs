using Raider.Game.Networking;

namespace Raider.Game.GUI.StartMenu
{
    public class StartMenuLobby : StartMenuPane
    {
        protected override void SetupPaneData()
        {
            NetworkGameManager.instance.UpdateLobbyNameplates();
        }

    }
}