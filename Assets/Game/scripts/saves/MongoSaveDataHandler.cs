using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

namespace Raider.Game.Saves
{
    /// <summary>
    /// Any data that may come from an API response. Parsed from JSON.
    /// </summary>
    [Serializable]
    public class ResponseObject
    {
        public bool success;
        public string message;
        public string token;

        public static ResponseObject ParseResponseJson(string jsonString)
        {
            return JsonUtility.FromJson<ResponseObject>(jsonString);
        }
    }

    public class MongoSaveDataHandler : ISaveDataHandler
    {
        private const string API_URL = "http://localhost:3000/api/";
        private string accessToken;

        public MongoSaveDataHandler(string username, string password, Action<bool, string> callback)
        {
            if (String.IsNullOrEmpty(accessToken))
                GetNewAccessToken(username, password, callback);
            else
                callback(true, "success");
        }

        public IEnumerator GetNewAccessToken(string username, string password, Action<bool, string> callback)
        {
            //Setup the form.
            WWWForm requestTokenForm = new WWWForm();
            requestTokenForm.AddField("username", username);
            requestTokenForm.AddField("password", password);

            //Send the request, with the form.
            WWW requestToken = new WWW(API_URL + "auth/register", requestTokenForm);
            yield return requestToken;

            //If there's any data in the error string, call the callback.
            if (!String.IsNullOrEmpty(requestToken.error))
                callback(false, requestToken.error);
            else
            {
                ResponseObject recievedResponse = ResponseObject.ParseResponseJson(requestToken.text);

                if (!recievedResponse.success)
                    callback(false, recievedResponse.message);
                else
                {
                    accessToken = recievedResponse.token;
                    callback(true, "success");
                }
            }

            //UnityWebRequest requestToken = new UnityWebRequest(API_URL + "/auth/login", "POST");
            //requestToken.
            //requestToken.SetRequestHeader("Authorization")
        }

        public SaveDataStructure ReadData()
        {
            throw new NotImplementedException();
        }

        public void SaveData(SaveDataStructure _data)
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            throw new NotImplementedException();
        }

        public void DeleteData()
        {
            throw new NotImplementedException();
        }

        public void NewData()
        {
            throw new NotImplementedException();
        }

        public void NewCharacter(SaveDataStructure.Character character)
        {
            throw new NotImplementedException();
        }

        public void SaveCharacter(int slot, SaveDataStructure.Character character)
        {
            throw new NotImplementedException();
        }

        public SaveDataStructure.Character GetCharacter(int slot)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<SaveDataStructure.Character> GetAllCharacters()
        {
            throw new NotImplementedException();
        }

        public string GetUsername()
        {
            throw new NotImplementedException();
        }

        public void SetUsername(string _username)
        {
            throw new NotImplementedException();
        }

        public int CharacterCount
        {
            get { throw new NotImplementedException(); }
        }

        public void DeleteCharacter(int slot)
        {
            throw new NotImplementedException();
        }

        public void DefaultSettings()
        {
            throw new NotImplementedException();
        }

        public void SaveSettings(SaveDataStructure.Settings settings)
        {
            throw new NotImplementedException();
        }

        public SaveDataStructure.Settings GetSettings()
        {
            throw new NotImplementedException();
        }
    }
}