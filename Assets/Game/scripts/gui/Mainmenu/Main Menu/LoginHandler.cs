using UnityEngine;
using UnityEngine.UI;

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
            Session.Login(usernameTextBox.text, passwordTextBox.text, LoginSuccessCallback, LoginFailedCallback);

            LoginPanel.gameObject.SetActive(false);
            loadingPanel.gameObject.SetActive(true);
        }

        public void LoginSuccessCallback(string message)
        {
            Session.userSaveDataHandler.ReloadData(LoadPlayerSuccessCallback, LoadPlayerFailedCallback);
            UserFeedback.LogError(message);
        }

        public void LoginFailedCallback(string error)
        {
            UserFeedback.LogError(error);
            ResetMenu();
        }

        public void LoadPlayerSuccessCallback(string message)
        {
            MainmenuController.instance.Login();

            ResetMenu();
        }

        public void LoadPlayerFailedCallback(string error)
        {
            UserFeedback.LogError(error);

            ResetMenu();
        }

        public void ResetMenu()
        {
            loadingPanel.gameObject.SetActive(false);
            LoginPanel.gameObject.SetActive(true);
        }
    }
}
