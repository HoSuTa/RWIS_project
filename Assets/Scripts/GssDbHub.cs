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
    }

    public class GssDbHub : MonoBehaviour
    {
        private string _gssUrl;
        private string _gasURL;
        private LocalGssDataManager _localGssData = new LocalGssDataManager();

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
        private bool _isRequestInProcess = false;

        private void Start()
        {
            _gasURL = KeyManager.GetKeyData(KeyManager.GAS_URL_PATH);
            _gssUrl = GssKeyManager.GetGssKey();
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

        private void GetAllDatasFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName : message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName} : \"{datas[i].message}\"\n");
                _uiText.text = string.Concat(_uiText.text, $"{messageJson.ToString()}.\n");
            }
            _localGssData.RefreshAllDatas(datas);
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



        public void CheckIfGssUrlValid(string gssUrl, Action saveKeyFeedBack = null, Action updateKeyRelatedUiFeedBack = null)
        {
            _isRequestInProcess = true;
            StartCoroutine(GssGetter.CheckIfGssUrlValid(_gasURL, gssUrl, response => GssUrlValidFeedBack((string)response, saveKeyFeedBack, updateKeyRelatedUiFeedBack) ));
        }
        private void GssUrlValidFeedBack(string response, Action saveKeyFeedBack = null, Action updateKeyRelatedUiFeedBack = null)
        {
            if (!response.Contains("Error"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
            }

            Debug.Log(response);

        }
    }
}


