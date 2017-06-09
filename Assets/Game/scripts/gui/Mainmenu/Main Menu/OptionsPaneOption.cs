using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{

	public class OptionsPaneOption : EventTrigger
    {
        OptionsPaneHandler optionsPaneHandler;

        public class OptionsPaneContents
        {
            public readonly string name;
            public readonly Sprite image;
            public readonly string description;

            public OptionsPaneContents(string _name, string _description)
            {
                name = _name;
                description = _description;
            }

            public OptionsPaneContents(string _name, string _description, Sprite _sprite)
            {
                name = _name;
                description = _description;
                image = _sprite;
            }

        }

        public OptionsPaneContents optionData;

        protected virtual void Start()
        {
            optionsPaneHandler = GetComponentInParent<OptionsPaneHandler>();

            if (optionData == null)
            {
                Debug.Log("OptionPaneOption instanced with no OptionPaneContents!");
                Destroy(this.transform);
            }

            transform.Find("Label").GetComponent<Text>().text = optionData.name;
            this.name = optionData.name;

            GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(OnClick));
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            HoverEvent();
            base.OnPointerEnter(eventData);
        }

        protected virtual void HoverEvent()
        {
            optionsPaneHandler.OptionHover(optionData);
        }

        protected virtual void EndHoverEvent()
        {
            optionsPaneHandler.OptionStopHover();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            EndHoverEvent();
            base.OnPointerExit(eventData);
        }

        public virtual void OnClick()
        {
            optionsPaneHandler.OptionClicked(this.name);
        }
    }
}