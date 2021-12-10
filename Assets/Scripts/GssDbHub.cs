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
        SaveUserData,
    }

    public class GssDbHub : MonoBehaviour
    {
        private string _gasURL = null;
        [SerializeField]
        private Text _uiText;
        [SerializeField]
        private string _gasURLjsonPos = "gasUrl.json";
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

        private void Start()
        {
            _gasURL = KeyManager.GetGasUrl(_gasURLjsonPos);
        }

        private void Update()
        {
            if (_sendRequest)
            {
                Debug.Log($"<color=blue>[GssDbHub]</color> Sending data to GAS...");

                if (_requestMethod == MethodNames.GetUserDatas)
                {
                    StartCoroutine(GssGetter.GetUserDatas(_gasURL, _userName, response => GetUserDatasFeedback((PayloadData[])response)));
                }
                else if (_requestMethod == MethodNames.GetUserNames)
                {
                    StartCoroutine(GssGetter.GetUserNames(_gasURL, response => GetUserNamesFeedback((PayloadData[])response)));
                }
                else if (_requestMethod == MethodNames.SaveUserData)
                {
                    string message = $"{{" +
                        $"\"areaId\" : {_areaId}, " +
                        $"\"vertexId\" : {_vertexId}, " +
                        $"\"lonLat\" : {JsonUtility.ToJson(_lonLat)}" +
                        $"}}";
                    StartCoroutine(GssPoster.SaveUserData(_gasURL, _userName, message));
                }
                _sendRequest = false;
            }
        }


        private void GetUserNamesFeedback(PayloadData[] datas)
        {
            _uiText.text = "userNames\n";
            for (int i = 0; i < datas.Length; i++)
            {
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}\n");
            }
        }

        private void GetUserDatasFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName : message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName} : \"{datas[i].message}\"\n");
                _uiText.text = string.Concat(_uiText.text, $"      areaId={messageJson.areaId}, " +
                                                            $"vertexId={messageJson.vertexId}, " +
                                                            $"lonLat={messageJson.lonLat}.\n");
            }
        }
    }
}


