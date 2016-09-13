using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {

    #region Singleton Setup

    public static MenuManager instance;

    public void Awake()
    {
        if (instance != null)
            Debug.LogAssertion("It seems that multiple Menu Managers are active, breaking the singleton instance.");
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    #endregion

    public Menu CurrentMenu;

    public void Start()
    {
        ShowMenu(CurrentMenu);
    }

    public void ShowMenu(Menu menu)
    {
        if(CurrentMenu != null)
        {
            CurrentMenu.IsOpen = false;
        }

        CurrentMenu = menu;
        CurrentMenu.IsOpen = true;
    }

}
