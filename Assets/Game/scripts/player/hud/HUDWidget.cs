using System;
using UnityEngine;

namespace Raider.Game.Player.HUD
{
    public class HUDWidget : MonoBehaviour
    {
		//[Flags]
		//public enum StateData : ushort
		//{
		//	None,
		//	IsAlive = 1 << 0,
		//	Bit1 = 1 << 1,
		//	Bit2 = 1 << 2,
		//	Bit3 = 1 << 3,
		//	Bit4 = 1 << 4,
		//	Bit5 = 1 << 5,
		//	Bit6 = 1 << 6,
		//	Bit7 = 1 << 7,
		//	Bit8 = 1 << 8,
		//	Bit9 = 1 << 9,
		//	Bit10 = 1 << 10,
		//	Bit11 = 1 << 11,
		//	Bit12 = 1 << 12,
		//	Bit13 = 1 << 13,
		//	Bit14 = 1 << 14,
		//	Bit15 = 1 << 15
		//}

		//public StateData stateData;

		//[HideInInspector]
  //      public HUDAnimationController animationController;

        //public bool WidgetEnabled
        //{
        //    get { return WidgetEnabled; }
        //    set
        //    {
        //        WidgetEnabled = value;

        //        if (value)
        //            ShowWidget();
        //        else
        //            HideWidget();
        //    }
        //}

        //Make sure to call the show or hide widget method.
        //void Start()
        //{
        //    animationController = GetComponent<HUDAnimationController>();
        //    WidgetEnabled = WidgetEnabled;
        //}

        //void ShowWidget()
        //{
        //    if (animationController != null)
        //        animationController.HudWidgetBoot();
        //    gameObject.SetActive(true);
        //}

        //void HideWidget()
        //{
        //    if (animationController != null)
        //        animationController.HudWidgetShutDown();
        //    gameObject.SetActive(false);
        //}
    }
}