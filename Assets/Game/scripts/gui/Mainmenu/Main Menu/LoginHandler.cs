using UnityEngine;
using System;
using Raider.Game.Scene;
using UnityEngine.UI;
using Raider.Game.GUI.Screens;

namespace Raider.Game.GUI.Screens
{
    public class LoginHandler : MonoBehaviour
    {
        [SerializeField]
        private Text usernameTextBox;
        private Text passwordTextBox;
        private Toggle rememberMeBox;
        private RectTransform LoginPanel;
        private RectTransform loadingPanel;

        //Sends username and password to session class to login.
        public void BeginLogin()
        {
            if (string.IsNullOrEmpty(usernameTextBox.text))
            {
                UserFeedback.LogError("No username provided.");
                //Display info.
                return;
            }

            if(string.IsNullOrEmpty(passwordTextBox.text) && Session.onlineMode)
            {
                UserFeedback.LogError("No password provided.");
                //Display info.
                return;
            }

            Session.rememberMe = rememberMeBox.isOn;
            Session.Login(usernameTextBox.text, passwordTextBox.text, LoginCallback);
        }

        public void LoginCallback(bool success, string error)
        {
            if (success)
                MainmenuHandler.instance.Login();
            else
                UserFeedback.LogError(error);

            loadingPanel.gameObject.SetActive(false);
            LoginPanel.gameObject.SetActive(true);
        }
    }
}
