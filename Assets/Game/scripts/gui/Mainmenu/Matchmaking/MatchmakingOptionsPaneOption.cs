using UnityEngine;
using System.Collections;
using Raider.Game.GUI.Screens;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{
    public class MatchmakingOptionsPaneOption : OptionsPaneOption
    {
        MatchmakingHandler matchmakingHandler;

        protected override void Start()
        {
            matchmakingHandler = GetComponentInParent<MatchmakingHandler>();

            if (optionData == null)
            {
                Debug.Log("OptionPaneOption instanced with no OptionPaneContents!");
                Destroy(this.transform);
            }

            transform.Find("Label").GetComponent<Text>().text = optionData.name;
            this.name = optionData.name;

            GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(OnClick));
        }

        public override void OnClick()
        {
            matchmakingHandler.OptionClicked(this.name);
        }
    }
}
