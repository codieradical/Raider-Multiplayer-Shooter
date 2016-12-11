using UnityEngine;
using System.Collections;

namespace Raider.Game.Player.HUD
{
    public class HUDWidget : MonoBehaviour
    {
        [HideInInspector]
        public HUDAnimationController animationController;

        public bool WidgetEnabled
        {
            get { return WidgetEnabled; }
            set
            {
                WidgetEnabled = value;

                if (value)
                    ShowWidget();
                else
                    HideWidget();
            }
        }

        //Make sure to call the show or hide widget method.
        void Start()
        {
            animationController = GetComponent<HUDAnimationController>();
            WidgetEnabled = WidgetEnabled;
        }

        void ShowWidget()
        {
            if (animationController != null)
                animationController.HudWidgetBoot();
            gameObject.SetActive(true);
        }

        void HideWidget()
        {
            if (animationController != null)
                animationController.HudWidgetShutDown();
            gameObject.SetActive(false);
        }
    }
}