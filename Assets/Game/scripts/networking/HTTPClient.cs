using Raider.Game.Saves.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

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

        private struct PendingDataCallback
        {
            public PendingDataCallback(ResponseData _data, Action<ResponseData> _callback)
            {
                data = _data;
                dataCallback = _callback;
            }
            public ResponseData data;
            public Action<ResponseData> dataCallback;
        }
        private struct PendingMessageCallback
        {
            public PendingMessageCallback(string _message, Action<string> _callback)
            {
                message = _message;
                messageCallback = _callback;
            }
            public string message;
            public Action<string> messageCallback;
        }

        private Queue<PendingDataCallback> pendingDataCallbacks = new Queue<PendingDataCallback>();
        private Queue<PendingMessageCallback> pendingMessageCallbacks = new Queue<PendingMessageCallback>();

        private void Update()
        {
            while (pendingDataCallbacks.Count > 0)
            {
                PendingDataCallback pendingCallback = pendingDataCallbacks.Dequeue();
                pendingCallback.dataCallback(pendingCallback.data);
            }
            while (pendingMessageCallbacks.Count > 0)
            {
                PendingMessageCallback pendingCallback = pendingMessageCallbacks.Dequeue();
                pendingCallback.messageCallback(pendingCallback.message);
            }
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
            Thread requestThread = new Thread(() => instance.HandleHTTPRequest(URL, method, successCallback, failureCallback, dataCallback, authorization, form));
            requestThread.Start();
        }

        private void HandleHTTPRequest(string URL, string method, Action<string> successCallback, Action<string> failureCallback, Action<ResponseData> dataCallback, string authorization, WWWForm form)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL);

            WebHeaderCollection headers = new WebHeaderCollection();

            webRequest.Method = method;

            //If a web token is provided, add it to the request header.
            if (authorization != null)
            {
                //headers.Set(HttpRequestHeader.Authorization, authorization);
                headers.Add("Authorization", authorization);
                //webRequest.Credentials = new CredentialCache();
                webRequest.PreAuthenticate = true;

            }

            webRequest.Headers = headers;

            try
            {
                if (form != null)
                {
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.ContentLength = form.data.Length;
                    Stream postDataStream = webRequest.GetRequestStream();
                    postDataStream.Write(form.data, 0, form.data.Length);
                    postDataStream.Close();
                }
            }
            catch (Exception ex) //A socket excaption can be thrown here, it can also be rethrown as a Web Exception.
            {
                if (failureCallback != null)
                    pendingMessageCallbacks.Enqueue(new PendingMessageCallback(ex.Message, failureCallback));
                return;
            }

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();

                Stream responseDataStream = response.GetResponseStream();

                StreamReader responseDataStreamReader = new StreamReader(responseDataStream);

                string responseJSON = responseDataStreamReader.ReadToEnd();

                responseDataStream.Close();
                responseDataStreamReader.Close();
                response.Close();

                ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJSON);

                pendingDataCallbacks.Enqueue(new PendingDataCallback(responseData, dataCallback));
                if (!responseData.success && failureCallback != null)
                    pendingMessageCallbacks.Enqueue(new PendingMessageCallback(responseData.message, failureCallback));
                if (responseData.success && successCallback != null)
                    pendingMessageCallbacks.Enqueue(new PendingMessageCallback(responseData.message, successCallback));
            }
            catch
            (Exception ex)
            {
                if(failureCallback != null)
                    pendingMessageCallbacks.Enqueue(new PendingMessageCallback(ex.Message, failureCallback));
            }
        }
    }
}
