using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GssDbManageWrapper
{
    [RequireComponent(typeof(GssDbHub))]
    public class GssDbHubTester : MonoBehaviour
    {
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
        private MessageJson[] _messageJsons;
        [SerializeField]
        private MethodNames _requestMethod = MethodNames.GetUserNames;
        [SerializeField]
        private bool _sendRequest = false;


        [SerializeField]
        private GssDbHub _gssDbHub;

        private void Start()
        {
            if (_gssDbHub == null)
            {
                _gssDbHub = GetComponent<GssDbHub>();
            }
        }

        private void Update()
        {
            ForTesting();
        }

        private void ForTesting()
        {
            if (_sendRequest)
            {
                Debug.Log($"<color=blue>[GssDbHubTester]</color> Sending data to GAS... method={_requestMethod}.");

                if (_requestMethod == MethodNames.GetAllDatas)
                {
                    _gssDbHub.GetAllDatas(GetAllDatasFeedback);
                }
                else if (_requestMethod == MethodNames.GetUserNames)
                {
                    _gssDbHub.GetUserNames(GetUserNamesFeedback);
                }
                else if (_requestMethod == MethodNames.GetUserDatas)
                {
                    _gssDbHub.GetUserDatas(_userName, GetUserDatasFeedback);
                }
                else if (_requestMethod == MethodNames.SaveData)
                {
                    _gssDbHub.SaveData(_userName, _areaId, _vertexId, _position);
                }
                else if (_requestMethod == MethodNames.SaveMultiple)
                {
                    _gssDbHub.SaveMultiple(_userName, _messageJsons);
                }
                else if (_requestMethod == MethodNames.RemoveData)
                {
                    _gssDbHub.RemoveData(_userName, _areaId, _vertexId);
                }
                _sendRequest = false;
            }
        }



        private void GetAllDatasFeedback(PayloadData[] datas)
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




    }
}


