using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Threading;
using Raider.Game.Networking;
using System.Text;

namespace Raider.Game.Saves.User
{
    public class MongoUserSaveDataHandler : IUserSaveDataHandler
    {
        private string accessToken;
        private UserSaveDataStructure data;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        public void Login(string username, string password, Action<string> successCallback, Action<string> failureCallback)
        {
            //if (String.IsNullOrEmpty(accessToken))
            //{
            //    if (!String.IsNullOrEmpty(Session.systemSaveDataHandler.GetToken()))
            //    {
            //        accessToken = Session.systemSaveDataHandler.GetToken();
            //        ReloadData(messageCallback);
            //    }
            //    else
                //{
                    GetNewAccessToken(username, password, successCallback, failureCallback);
            //    }
            //}
            //else
            //{
            //    ReloadData(messageCallback);
            //}
        }

        public void GetNewAccessToken(string username, string password, Action<string> successCallback, Action<string> failureCallback)
        {
            WWWForm requestForm = new WWWForm();
            requestForm.AddField("username", username);
            requestForm.AddField("password", password);
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/auth/login", "POST", RecieveTokenResponse, successCallback, failureCallback, null, requestForm);

        }
        private void RecieveTokenResponse(HTTPClient.ResponseData responseData)
        {
            accessToken = responseData.token;
            if (Session.rememberMe)
                Session.systemSaveDataHandler.SetToken(responseData.token);
            else
                Session.systemSaveDataHandler.DeleteToken();
        }

        public UserSaveDataStructure ReadData()
        {
            return data;
        }

        public void ReloadData(Action<string> successCallback, Action<string> failureCallback)
        {
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/user/", "GET", RecieveReloadDataResponse, successCallback, failureCallback, accessToken, null);
        }
        private void RecieveReloadDataResponse(HTTPClient.ResponseData response)
        {
            if (response.success)
                data = response.user;
        }

        public void DeleteData()
        {
            
        }

        public void NewData()
        {
            //The NewData method should not be called on the database.
            //Consider interface removal.
            
        }

        public void NewCharacter(UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            WWWForm requestForm = new WWWForm();
            requestForm.AddField("character", JsonUtility.ToJson(character));
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/user/characters/new", "POST", RecieveSaveCharacterResponse, successCallback, failureCallback, accessToken, requestForm);
        }

        public void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<string> successCallback, Action<string> failureCallback)
        {
            WWWForm requestForm = new WWWForm();
            requestForm.AddField("character", JsonUtility.ToJson(character));
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/user/characters/" + slot, "PUT", RecieveSaveCharacterResponse, successCallback, failureCallback, accessToken, requestForm);
        }
        private void RecieveSaveCharacterResponse(HTTPClient.ResponseData response)
        {
            if (response.success)
                ReloadData(null, null);
        }

        public UserSaveDataStructure.Character GetCharacter(int slot)
        {
            return data.characters[slot];
        }

        public List<UserSaveDataStructure.Character> GetAllCharacters()
        {
            return data.characters;
        }

        public string GetUsername()
        {
            return data.username;
        }

        public void SetUsername(string _username, Action<string> successCallback, Action<string> failureCallback)
        {
            WWWForm requestForm = new WWWForm();
            requestForm.AddField("username", _username);
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/user/username", "PUT", RecieveSetUsernameResponse, successCallback, failureCallback, accessToken, requestForm);
        }
        private void RecieveSetUsernameResponse(HTTPClient.ResponseData response)
        {
            if (response.success)
                ReloadData(null, null);
        }

        public int CharacterCount
        {
            get { return data.characters.Count; }
        }

        public void DeleteCharacter(int slot, Action<string> successCallback, Action<string> failureCallback)
        {
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/user/characters/" + slot, "DEL", RecieveDeleteCharacterResponse, successCallback, failureCallback, accessToken, null);
        }
        private void RecieveDeleteCharacterResponse(HTTPClient.ResponseData response)
        {
            if (response.success)
                ReloadData(null, null);
        }

        public void DefaultSettings(Action<string> successCallback, Action<string> failureCallback)
        {
            SaveSettings(new UserSaveDataStructure.UserSettings(), successCallback, failureCallback);
        }

        public void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<string> successCallback, Action<string> failureCallback)
        {
            WWWForm requestForm = new WWWForm();
            requestForm.AddField("settings", JsonUtility.ToJson(settings));
            HTTPClient.BeginHTTPRequest(BuildConfig.API_URL + "/user/settings", "PUT", RecieveSaveSettingsResponse, successCallback, failureCallback, accessToken, requestForm);
        }
        private void RecieveSaveSettingsResponse(HTTPClient.ResponseData response)
        {
            if (response.success)
                ReloadData(null, null);
        }

        public UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }
    }
}