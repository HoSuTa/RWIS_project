using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GssDbManageWrapper
{
    public enum MethodNames
    {
        GetUserNames,
        GetUserDatas,
        GetAllDatas,
        SaveMessage,
        SaveLonLat,
        RemoveData,
        CheckIfGssUrlValid,
        CheckIfGasUrlValid,
    }

    public class GssDbHub : MonoBehaviour
    {
        private string _gssUrl;
        private string _gasURL;
        public LocalGssDataManager _localGssData = new LocalGssDataManager();

        [SerializeField]
        private Text _uiText;

        [SerializeField]
        private string _userName = "tester";
        [SerializeField]
        private int _areaId = 0;
        [SerializeField]
        private int _vertexId = 0;
        [SerializeField]
        private Vector2 _lonLat = new Vector2(0,0);

        [SerializeField]
        private MethodNames _requestMethod = MethodNames.GetUserNames;
        [SerializeField]
        private bool _sendRequest = false;

        private bool _isGssUrlValid = false;

        private void Start()
        {
            _gasURL = GasUrlManager.GetUrl();
            _gssUrl = GssUrlManager.GetUrl();
        }

        private void Update()
        {
            if (_sendRequest)
            {
                Debug.Log($"<color=blue>[GssDbHub]</color> Sending data to GAS...");

                if (_requestMethod == MethodNames.GetAllDatas)
                {
                    StartCoroutine(GssGetter.GetAllDatas(_gasURL, _gssUrl, response => GetAllDatasFeedback((PayloadData[])response)));
                }
                else if (_requestMethod == MethodNames.GetUserDatas)
                {
                    StartCoroutine(GssGetter.GetUserDatas(_gasURL, _gssUrl, _userName, response => GetUserDatasFeedback((PayloadData[])response)));
                }
                else if (_requestMethod == MethodNames.GetUserNames)
                {
                    StartCoroutine(GssGetter.GetUserNames(_gasURL, _gssUrl, response => GetUserNamesFeedback((PayloadData[])response)));
                }
                else if (_requestMethod == MethodNames.CheckIfGssUrlValid)
                {
                    StartCoroutine(GssGetter.CheckIfGssUrlValid(_gasURL, _gssUrl, response => GssUrlValidFeedBack((string)response) ) );
                }
                else if (_requestMethod == MethodNames.CheckIfGasUrlValid)
                {
                    StartCoroutine(GssGetter.CheckIfGasUrlValid(_gasURL, response => GasUrlValidFeedBack((string)response)));
                }
                else if (_requestMethod == MethodNames.SaveMessage)
                {
                    string message = $"{{" +
                        $"\"areaId\" : {_areaId}, " +
                        $"\"vertexId\" : {_vertexId}, " +
                        $"\"lonLat\" : {JsonUtility.ToJson(_lonLat)}" +
                        $"}}";
                    StartCoroutine(GssPoster.SaveUserData(_gasURL, _gssUrl, _userName, message));
                }
                else if (_requestMethod == MethodNames.RemoveData)
                {
                    string message = $"{{" +
                        $"\"areaId\" : {_areaId}, " +
                        $"\"vertexId\" : {_vertexId} " +
                        $"}}";
                    StartCoroutine(GssPoster.RemoveData(_gasURL, _gssUrl, _userName, message));
                }
                _sendRequest = false;
            }
        }

        private void SaveData(string areaId, string vertexId, string lonLat)
        {
            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId}, " +
                        $"\"lonLat\" : {JsonUtility.ToJson(lonLat)}" +
                        $"}}";
            StartCoroutine(GssPoster.SaveUserData(_gasURL, _gssUrl, _userName, message));
        }

        private void RemoveData(string areaId, string vertexId)
        {
            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId} " +
                        $"}}";
            StartCoroutine(GssPoster.RemoveData(_gasURL, _gssUrl, _userName, message));
        }

        private void GetAllDatas(Action<object> localySaveAllDatasFeedBack = null)
        {
            StartCoroutine(GssGetter.GetAllDatas(_gasURL, _gssUrl, response => GetAllDatasFeedback((PayloadData[])response, localySaveAllDatasFeedBack)));
        }

        private void GetAllDatasFeedback(PayloadData[] datas, Action<object> localySaveAllDatasFeedBack = null)
        {
            _uiText.text = "userName : message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName} : \"{datas[i].message}\"\n");
                _uiText.text = string.Concat(_uiText.text, $"{messageJson.ToString()}.\n");
            }
            _localGssData.RefreshAllDatas(datas);
            localySaveAllDatasFeedBack?.Invoke(_localGssData);
        }

        private void GetUserDatasFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName : message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName} : \"{datas[i].message}\"\n");
                _uiText.text = string.Concat(_uiText.text, $"{messageJson.ToString()}.\n");
            }
            _localGssData.RefreshUserDatas(datas);
        }

        private void GetUserNamesFeedback(PayloadData[] datas)
        {
            _uiText.text = "userNames\n";
            for (int i = 0; i < datas.Length; i++)
            {
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}\n");
            }
            _localGssData.RefreshUserNames(datas);
            foreach(var userName in _localGssData._userNames)
            {
                Debug.Log(userName);
            }
        }



        public void CheckIfGssUrlValid
        (
            string gssUrl, 
            Action saveKeyFeedBack = null, 
            Action updateKeyRelatedUiFeedBack = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGssUrlValid
                (
                    _gasURL, 
                    gssUrl, 
                    response => GssUrlValidFeedBack
                    (
                        (string)response, 
                        saveKeyFeedBack, 
                        updateKeyRelatedUiFeedBack,
                        invalidFeedback
                    ) 
                )
            );
        }

        private void GssUrlValidFeedBack
        (
            string response, 
            Action saveKeyFeedBack = null, 
            Action updateKeyRelatedUiFeedBack = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Error"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }


        public void CheckIfGasUrlValid
        (
            string gasUrl,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGasUrlValid
                (
                    gasUrl,
                    response => GssUrlValidFeedBack
                    (
                        (string)response,
                        saveKeyFeedBack,
                        updateKeyRelatedUiFeedBack,
                        invalidFeedback
                    )
                )
            );
        }

        private void GasUrlValidFeedBack
        (
            string response, 
            Action saveKeyFeedBack = null, 
            Action updateKeyRelatedUiFeedBack = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Cannot"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }
    }
}


