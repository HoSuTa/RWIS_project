﻿using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public static class GssPoster
    {
        public static IEnumerator SaveUserData(string gasUrl, string gssUrl, string userName, string message, Action<object> feedbackHandler = null)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.SaveData}\" , \"gssUrl\" : \"{gssUrl}\", \"userName\" : \"{userName}\", \"message\" : {message} }}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.SaveData, payloadRaw, feedbackHandler);
        }
        public static IEnumerator UpdateMultipleDatas(string gasUrl, string gssUrl, string userName, string message, Action<object> feedbackHandler = null)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.UpdateMultiple}\" , \"gssUrl\" : \"{gssUrl}\", \"userName\" : \"{userName}\", \"message\" : {message} }}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.UpdateMultiple, payloadRaw, feedbackHandler);
        }

        public static IEnumerator RemoveData(string gasUrl, string gssUrl, string userName, string message, Action<object> feedbackHandler = null)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.RemoveData}\" , \"gssUrl\" : \"{gssUrl}\", \"userName\" : \"{userName}\", \"message\" : {message} }}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.RemoveData, payloadRaw, feedbackHandler);
        }

        private static IEnumerator PostToGss(string gasUrl, MethodNames methodName, byte[] payload, Action<object> feedbackHandler = null)
        {
            UnityWebRequest request =
                (methodName == MethodNames.SaveData) ?
                    UnityWebRequest.Post($"{gasUrl}", "POST")
                : (methodName == MethodNames.UpdateMultiple) ?
                    UnityWebRequest.Post($"{gasUrl}", "POST")
                : (methodName == MethodNames.RemoveData) ?
                    UnityWebRequest.Post($"{gasUrl}", "POST")
                : null;
            if (request == null)
            {
                Debug.LogError($"<color=blue>[GssPoster]</color> Behaviour for \"{methodName}\" is not implemented.");
                yield break;
            }


            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(payload);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();


            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError($"<color=blue>[GssPoster]</color> Sending data to GAS failed. Error: {request.error}");
            }
            else
            {
                var request_result = request.downloadHandler.text;

                feedbackHandler?.Invoke(request_result);
                yield break;
            }
        }


    }
}
