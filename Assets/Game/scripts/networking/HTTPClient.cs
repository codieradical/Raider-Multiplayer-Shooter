using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Raider.Game.Saves.User;
using System.Collections;
using System.Net;
using System.IO;

namespace Raider.Game.Networking
{
    public class HTTPClient : MonoBehaviour
    {
        #region singleton setup

        private static HTTPClient instance;

        void Awake()
        {
            if (instance != null)
                Debug.LogError("More than one HTTPClient are active! What are you doing!!");
            instance = this;
        }

        void OnDestroy()
        {
#if !UNITY_EDITOR
            Debug.LogError("Something just destroyed the HTTPClient!");
#endif
            instance = null;
        }

        #endregion

        public class ResponseData
        {
            public UserSaveDataStructure user;
            public UserSaveDataStructure.Character character;
            //Defaults to error.
            public bool success = false;
            public string message;
            public string token;
        }

        /// <summary>
        /// Passes parameters to a local coroutine and begins a web request.
        /// </summary>
        /// <param name="URL">The URL to request/</param>
        /// <param name="method">The request method, eg GET, POST, PUT.</param>
        /// <param name="dataCallback">A callback to act on data recieved. Not called if an error occurs.</param>
        /// <param name="successCallback">Optional, called if the request was successful.</param>
        /// <param name="failureCallback">Optional, called if the request failed.</param>
        /// <param name="authorization">Optional, and Authorization</param>
        /// <param name="form">Optional, POST or PUT data.</param>
        public static void BeginHTTPRequest(string URL, string method, Action<ResponseData> dataCallback, Action<string> successCallback, Action<string> failureCallback, string authorization, WWWForm form)
        {
            instance.StartCoroutine(instance.HandleHTTPRequest(URL, method, successCallback, failureCallback, dataCallback, authorization, form));
        }

        private IEnumerator HandleHTTPRequest(string URL, string method, Action<string> successCallback, Action<string> failureCallback, Action<ResponseData> dataCallback, string authorization, WWWForm form)
        {
            WebRequest webRequest = WebRequest.Create(URL);

            WebHeaderCollection headers = new WebHeaderCollection();

            webRequest.Method = method;

            //If a web token is provided, add it to the request header.
            if (authorization != null)
                headers.Set(HttpRequestHeader.Authorization, authorization);

            headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

            if (form != null)
            {
                Stream postDataStream = webRequest.GetRequestStream();
                postDataStream.Write(form.data, 0, form.data.Length);
                postDataStream.Close();
            }

            WebResponse response;

            yield return response = webRequest.GetResponse();

            Stream responseDataStream = response.GetResponseStream();

            StreamReader responseDataStreamReader = new StreamReader(responseDataStream);

            string responseJSON = responseDataStreamReader.ReadToEnd();

            responseDataStream.Close();
            responseDataStreamReader.Close();
            response.Close();

            ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJSON);
            if (!responseData.success && failureCallback != null)
                failureCallback(responseData.message);
            if (successCallback != null)
                successCallback(responseData.message);
            dataCallback(responseData);
        }
    }
}
