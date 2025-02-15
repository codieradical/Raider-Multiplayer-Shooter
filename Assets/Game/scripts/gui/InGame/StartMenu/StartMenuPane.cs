﻿using UnityEngine;

namespace Raider.Game.GUI.StartMenu
{
	[RequireComponent(typeof(Animator))]
    public abstract class StartMenuPane : MonoBehaviour
    {
        public bool IsOpen
        {
            get { return animatorInstance.GetBool("open"); }
            set { animatorInstance.SetBool("open", value); }
        }

        Animator animatorInstance;

        protected virtual void Awake()
        {
            animatorInstance = GetComponent<Animator>();
            enabled = false;
        }

        public void OpenPane()
        {
            SetupPaneData();
            IsOpen = true;
        }

        protected abstract void SetupPaneData();

        public virtual void ClosePane()
        {
            IsOpen = false;
        }
    }
}