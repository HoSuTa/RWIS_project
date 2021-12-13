﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public class GssGetter
    {
        public static IEnumerator GetAllDatas(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetAllDatas, "", feedbackHandler);
        }
        public static IEnumerator GetUserDatas(string gasUrl, string gssUrl, string userName, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetUserDatas, userName, feedbackHandler);
        }

        public static IEnumerator GetUserNames(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetUserNames, "", feedbackHandler);
        }

        public static IEnumerator CheckIfGssUrlValid(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.CheckIfGssUrlValid, "", feedbackHandler);
        }

        private static IEnumerator GetGssData(string gasUrl, string gssUrl, MethodNames methodName, string userName, Action<object> feedbackHandler = null)
        {
            UnityWebRequest request =
                (methodName == MethodNames.GetAllDatas) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(gssUrl)}={gssUrl}")
                : (methodName == MethodNames.GetUserDatas) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(gssUrl)}={gssUrl}&{nameof(userName)}={userName}")
                : (methodName == MethodNames.GetUserNames) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(gssUrl)}={gssUrl}") 
                : (methodName == MethodNames.CheckIfGssUrlValid) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(gssUrl)}={gssUrl}")
                : null;
            if(request == null)
            {
                Debug.LogError($"<color=blue>[GssGetter]</color> Behaviour for \"{methodName}\" is not implemented.");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                Debug.LogError($"<color=blue>[GssGetter]</color> Sending data to GAS failed. Error: {request.error}.");
                yield break;
            }
            else
            {
                var request_result = request.downloadHandler.text;

                if (request_result.Contains("Error"))
                {
                    Debug.LogError($"<color=blue>[GssGetter]</color> {request_result} ");
                    yield break;
                }

                if (methodName == MethodNames.CheckIfGssUrlValid)
                {
                    feedbackHandler?.Invoke(request_result);
                }
                else
                {
                    var response = JsonExtension.FromJson<PayloadData>(request_result);
                    if (methodName == MethodNames.GetAllDatas)
                    {
                        feedbackHandler?.Invoke(response);
                    }
                    else if(methodName == MethodNames.GetUserNames)
                    {
                        feedbackHandler?.Invoke(response);
                    }
                    else if (methodName == MethodNames.GetUserDatas)
                    {
                        feedbackHandler?.Invoke(response);
                    } 
                }
            }
        }
    }
}
