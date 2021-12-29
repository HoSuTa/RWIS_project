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
        RemoveData,
        CheckIfGssUrlValid,
        CheckIfGasUrlValid,
    }

    public class GssDbHub : MonoBehaviour
    {
        private string _gssUrl;
        private string _gasURL;
        public LocalGssDataManager _localGssData = new LocalGssDataManager();

        [Header("テスト用のパラメータ")]
        [SerializeField]
        private Text _uiText;
        [SerializeField]
        private string _userName = "tester";
        [SerializeField]
        private int _areaId = 0;
        [SerializeField]
        private int _vertexId = 0;
        [SerializeField]
        private Vector3 _position = new Vector3(0,0,0);
        [SerializeField]
        private MethodNames _requestMethod = MethodNames.GetUserNames;
        [SerializeField]
        private bool _sendRequest = false;


        private void Start()
        {
            _gasURL = GasUrlManager.GetUrl();
            _gssUrl = GssUrlManager.GetUrl();
        }

        private void Update()
        {
            ForTesting();
        }

        private void ForTesting()
        {
            if (_sendRequest)
            {
                Debug.Log($"<color=blue>[GssDbHub]</color> Sending data to GAS... method={_requestMethod}.");

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
                    StartCoroutine(GssGetter.CheckIfGssUrlValid(_gasURL, _gssUrl, response => GssUrlValidFeedBack((string)response)));
                }
                else if (_requestMethod == MethodNames.CheckIfGasUrlValid)
                {
                    StartCoroutine(GssGetter.CheckIfGasUrlValid(_gasURL, response => GasUrlValidFeedBack((string)response)));
                }
                else if (_requestMethod == MethodNames.SaveMessage)
                {
                    SaveData(_userName, _areaId, _vertexId, _position);
                }
                else if (_requestMethod == MethodNames.RemoveData)
                {
                    RemoveData(_userName, _areaId, _vertexId);
                }
                _sendRequest = false;
            }
        }

        private void PostFeedBack(string response)
        {
            Debug.Log(response);
        }

        public void SaveData(string userName, int areaId, int vertexId, Vector3 position)
        {
            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId}, " +
                        $"\"position\" : {JsonUtility.ToJson(position)}" +
                        $"}}";
            StartCoroutine(
                GssPoster.SaveUserData(_gasURL, _gssUrl, userName, message, response => PostFeedBack((string)response)));
        }

        public void RemoveData(string userName, int areaId, int vertexId)
        {
            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId} " +
                        $"}}";
            StartCoroutine(
                GssPoster.RemoveData(_gasURL, _gssUrl, userName, message, response => PostFeedBack((string)response)));
        }

        private void GetAllDatas(Action<object> useLocalDataFeedBack = null)
        {
            StartCoroutine(GssGetter.GetAllDatas(_gasURL, _gssUrl, response => GetAllDatasFeedback((PayloadData[])response, useLocalDataFeedBack)));
        }

        private void GetAllDatasFeedback(PayloadData[] datas, Action<object> useLocalData = null)
        {
            _uiText.text = "userName: message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}:\n");
                _uiText.text = string.Concat(_uiText.text, $"{messageJson.ToString()}.\n");
            }
            _localGssData.RefreshAllDatas(datas);
            useLocalData?.Invoke(_localGssData);
        }

        private void GetUserDatasFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName: message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}:\n");
                _uiText.text = string.Concat(_uiText.text, $"{messageJson.ToString()}.\n");
            }
        }

        private void GetUserNamesFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName:\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}\n");
            }
            _localGssData.RefreshAllDatas(datas);
        }

        public void CheckIfGssUrlValid
        (
            string gssUrl, 
            Action saveKeyFeedBack = null, 
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
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
                        validFeedback,
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
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Error"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
                validFeedback?.Invoke();
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
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGasUrlValid
                (
                    gasUrl,
                    response => GasUrlValidFeedBack
                    (
                        (string)response,
                        saveKeyFeedBack,
                        updateKeyRelatedUiFeedBack,
                        validFeedback,
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
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Cannot"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
                validFeedback?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }
    }
}


