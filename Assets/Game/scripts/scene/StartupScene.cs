using UnityEngine;

namespace Raider.Game.Scene
{

    public class StartupScene : MonoBehaviour {

        public string startScene = "mainmenu";

        //On the first frame, the scenario singleton is assigned.
        //On the second frame, load the other scenario.
        //There will be no third frame.
        void Update() {
            Scenario.instance.LoadScene(startScene, Scenario.Gametype.Ui);
        }
    }
}