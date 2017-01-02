using Raider.Game.Saves.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Networking
{
    public class ApiWebRequestHandler
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
            public UserSaveDataStructure user;
            public UserSaveDataStructure.Character character;

            public static ResponseObject ParseResponseJson(string jsonString)
            {
                return JsonUtility.FromJson<ResponseObject>(jsonString);
            }
        }

        public class ResponseHandler : DownloadHandlerScript
        {
            public ResponseHandler(Action<ResponseObject> _completedCallback) : base()
            {
                responseCallback = _completedCallback;
            }
            public ResponseHandler(Action<ResponseObject> _completedCallback, Action<bool, string> _messageCallback) : base()
            {
                responseCallback = _completedCallback;
                messageCallback = _messageCallback;
            }
            public ResponseHandler(Action<bool, string> _messageCallback) : base()
            {
                messageCallback = _messageCallback;
            }

            Action<ResponseObject> responseCallback;
            Action<bool, string> messageCallback;
            ResponseObject responseObject;

            protected override void CompleteContent()
            {
                //The text property is not ready yet.
                responseObject = ResponseObject.ParseResponseJson(Encoding.UTF8.GetString(data));
                if (responseCallback != null)
                    responseCallback(responseObject);
                if (messageCallback != null)
                    messageCallback(responseObject.success, responseObject.message);
            }

            byte[] rawData = new byte[] { };
            int rawDataLength = -1;

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (data.Length < 1)
                { }
                else if (rawData.Length < 1)
                {
                    rawData = data;
                }
                else
                {
                    rawData.Concat(data);
                }

                //if (rawData.Count() == rawDataLength)
                //    return false;
                //else
                    return true;
            }
            protected override void ReceiveContentLength(int contentLength)
            {
                rawDataLength = contentLength;
            }
            protected override byte[] GetData()
            {
                return rawData;
            }
        }

        /// <summary>
        /// Inheriting from the Upload Handler is impossible as construction is sealed.
        /// So, instead I'm creating a wrapper class.
        /// </summary>
        public class RequestHandler
        {
            public UploadHandlerRaw uploadHandler;
            public RequestHandler(WWWForm form)
            {
                uploadHandler = new UploadHandlerRaw(form.data);
                uploadHandler.contentType = "application/x-www-form-urlencoded";
            }

            public RequestHandler(WWWForm form, string contentType)
            {
                uploadHandler = new UploadHandlerRaw(form.data);
                uploadHandler.contentType = contentType;
            }
        }
    }
}
