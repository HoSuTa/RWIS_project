using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public class GssGetter
    {
        public static IEnumerator GetUserDatas(string gasUrl, string gssUrl, string userName, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetUserDatas, userName, feedbackHandler);
        }

        public static IEnumerator GetUserNames(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetUserNames, "", feedbackHandler);
        }

        public static IEnumerator IsGssKeyValid(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.IsGssKeyValid, "", feedbackHandler);
        }

        private static IEnumerator GetGssData(string gasUrl, string gssUrl, MethodNames methodName, string userName, Action<object> feedbackHandler = null)
        {
            UnityWebRequest request = 
                (methodName == MethodNames.GetUserNames) ?
                    request = UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(gssUrl)}={gssUrl}") 
                : (methodName == MethodNames.GetUserDatas) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(userName)}={userName}&{nameof(gssUrl)}={gssUrl}")
                : (methodName == MethodNames.IsGssKeyValid) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(userName)}={userName}&{nameof(gssUrl)}={gssUrl}")
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

                if (request_result[0] == 'E')
                {
                    Debug.Log(request_result);
                    yield break;
                }
                else
                {
                    var response = JsonExtension.FromJson<PayloadData>(request_result);

                    if(methodName == MethodNames.GetUserNames)
                    {
                        feedbackHandler?.Invoke(response);

                        for (int i = 0; i < response.Length; i++)
                        {
                            Debug.Log($"response[{i}].userName : {response[i].userName}");
                        }
                    }
                    else if (methodName == MethodNames.GetUserDatas)
                    {
                        feedbackHandler?.Invoke(response);

                        for (int i = 0; i < response.Length; i++)
                        {
                            if(response[i].message == null)
                            {
                                Debug.LogError($"<color=blue>[GssGetter]</color> response[i].message is null.");
                            }
                            Debug.Log($"[{i}] {response[i].ToString()}");
                        }
                    } 
                    else if (methodName == MethodNames.IsGssKeyValid)
                    {
                        feedbackHandler?.Invoke(response);
                        Debug.Log(request_result);
                    }
                }
            }
        }
    }
}
