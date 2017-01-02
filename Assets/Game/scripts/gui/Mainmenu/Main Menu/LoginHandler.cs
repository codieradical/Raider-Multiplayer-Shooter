using UnityEngine;
using System;
using Raider.Game.Scene;
using UnityEngine.UI;
using Raider.Game.GUI.Screens;
using Raider.Game.Saves.User;

namespace Raider.Game.GUI.Screens
{
    public class LoginHandler : MonoBehaviour
    {
        [SerializeField]
        private InputField usernameTextBox;
        [SerializeField]
        private InputField passwordTextBox;
        [SerializeField]
        private Toggle rememberMeBox;
        [SerializeField]
        private RectTransform LoginPanel;
        [SerializeField]
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

            if(string.IsNullOrEmpty(passwordTextBox.text) && BuildConfig.ONLINE_MODE)
            {
                UserFeedback.LogError("No password provided.");
                //Display info.
                return;
            }

            Session.rememberMe = rememberMeBox.isOn;
            Session.Login(usernameTextBox.text, passwordTextBox.text, LoginCallback);

            LoginPanel.gameObject.SetActive(false);
            loadingPanel.gameObject.SetActive(true);
        }

        public void LoginCallback(bool success, string message)
        {
            if (success)
                Session.userSaveDataHandler.ReloadData(LoadPlayerCallback);
            else
            {
                UserFeedback.LogError(message);
                loadingPanel.gameObject.SetActive(false);
                LoginPanel.gameObject.SetActive(true);
            }
        }

        public void LoadPlayerCallback(bool success, string message)
        {
            if (success)
                MainmenuHandler.instance.Login();
            else
                UserFeedback.LogError(message);

            loadingPanel.gameObject.SetActive(false);
            LoginPanel.gameObject.SetActive(true);
        }
    }
}
