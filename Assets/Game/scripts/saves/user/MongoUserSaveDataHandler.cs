using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Threading;
using Raider.Game.Networking;

namespace Raider.Game.Saves.User
{
    public class MongoUserSaveDataHandler : IUserSaveDataHandler
    {
        private string accessToken;
        private UserSaveDataStructure data;

        public void Login(string username, string password, Action<bool, string> messageCallback)
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
                    GetNewAccessToken(username, password, messageCallback);
            //    }
            //}
            //else
            //{
            //    ReloadData(messageCallback);
            //}
        }

        public void GetNewAccessToken(string username, string password, Action<bool,string> messageCallback)
        {
            SendTokenRequest(username, password, messageCallback);
        }
        private void SendTokenRequest(string username, string password, Action<bool, string> messageCallback)
        {
            WWWForm requestTokenForm = new WWWForm();
            requestTokenForm.AddField("username", username);
            requestTokenForm.AddField("password", password);

            ApiWebRequestHandler.RequestHandler requestHandler = new ApiWebRequestHandler.RequestHandler(requestTokenForm);
            ApiWebRequestHandler.ResponseHandler responseHandler = new ApiWebRequestHandler.ResponseHandler(RecieveTokenResponse, messageCallback);
            UnityWebRequest requestToken = new UnityWebRequest(BuildConfig.API_URL + "/auth/login", "POST", responseHandler, requestHandler.uploadHandler);
            requestToken.Send();
        }
        private void RecieveTokenResponse(ApiWebRequestHandler.ResponseObject response)
        {
            if(response.success)
            {
                accessToken = response.token;
                if (Session.rememberMe)
                    Session.systemSaveDataHandler.SetToken(response.token);
            }
        }

        public UserSaveDataStructure ReadData()
        {
            return data;
        }

        //requestToken.SetRequestHeader("Authorization")

        public void SaveData(UserSaveDataStructure _data, Action<bool, string> messageCallback)
        {
            SendSaveDataRequest(_data, messageCallback);
        }
        private void SendSaveDataRequest(UserSaveDataStructure user, Action<bool, string> messageCallback)
        {
            WWWForm postUserForm = new WWWForm();
            postUserForm.AddField("user", JsonUtility.ToJson(user));

            Debug.Log(JsonUtility.ToJson(user));

            ApiWebRequestHandler.ResponseHandler responseHandler = new ApiWebRequestHandler.ResponseHandler(RecieveSaveDataResponse, messageCallback);
            ApiWebRequestHandler.RequestHandler requestHandler = new ApiWebRequestHandler.RequestHandler(postUserForm);
            UnityWebRequest requestToken = new UnityWebRequest(BuildConfig.API_URL + "/user", "PUT", responseHandler, requestHandler.uploadHandler);
            requestToken.SetRequestHeader("Authorization", accessToken);
            requestToken.Send();
        }
        private void RecieveSaveDataResponse(ApiWebRequestHandler.ResponseObject response)
        {
            if (response.success)
            {
                ReloadData(null);
            }
        }

        public void ReloadData(Action<bool, string> messageCallback)
        {
            SendReloadDataRequest(messageCallback);
        }
        private void SendReloadDataRequest(Action<bool, string> messageCallback)
        {
            ApiWebRequestHandler.ResponseHandler responseHandler = new ApiWebRequestHandler.ResponseHandler(RecieveReloadDataResponse, messageCallback);
            UnityWebRequest requestToken = new UnityWebRequest(BuildConfig.API_URL + "/user", "GET");
            requestToken.downloadHandler = responseHandler;
            requestToken.SetRequestHeader("Authorization", accessToken);
            requestToken.Send();
        }
        private void RecieveReloadDataResponse(ApiWebRequestHandler.ResponseObject response)
        {
            if (response.success)
            {
                data = response.user;
            }
        }

        public void DeleteData()
        {
            
        }

        public void NewData()
        {
            //The NewData method should not be called on the database.
            //Consider interface removal.
            
        }

        public void NewCharacter(UserSaveDataStructure.Character character, Action<bool, string> messageCallback)
        {
            data.characters.Add(new UserSaveDataStructure.Character());
            SaveData(data, messageCallback);
        }

        public void SaveCharacter(int slot, UserSaveDataStructure.Character character, Action<bool, string> messageCallback)
        {
            data.characters[slot] = character;
            SaveData(data, messageCallback);
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

        public void SetUsername(string _username, Action<bool, string> messageCallback)
        {
            data.username = _username;
            SaveData(data, messageCallback);
        }

        public int CharacterCount
        {
            get { return data.characters.Count; }
        }

        public void DeleteCharacter(int slot, Action<bool, string> messageCallback)
        {
            data.characters.RemoveAt(slot);
            SaveData(data, messageCallback);
        }

        public void DefaultSettings(Action<bool, string> messageCallback)
        {
            data.userSettings = new UserSaveDataStructure.UserSettings();
            SaveData(data, messageCallback);
        }

        public void SaveSettings(UserSaveDataStructure.UserSettings settings, Action<bool, string> messageCallback)
        {
            data.userSettings = settings;
            SaveData(data, messageCallback);
        }

        public UserSaveDataStructure.UserSettings GetSettings()
        {
            return data.userSettings;
        }
    }
}