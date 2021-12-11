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
        SaveMessage,
        SaveLonLat,
        RemoveData,
    }

    public class GssDbHub : MonoBehaviour
    {
        private const string _gasURLjsonPos = "Assets/Resources/gasUrl.json";
        private string _gasURL;

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

        private LocalGssDataManager _localGssData = new LocalGssDataManager();

        private void Start()
        {
            _gasURL = KeyManager.GetKeyData(_gasURLjsonPos);
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
                else if (_requestMethod == MethodNames.SaveMessage)
                {
                    string message = $"{{" +
                        $"\"areaId\" : {_areaId}, " +
                        $"\"vertexId\" : {_vertexId}, " +
                        $"\"lonLat\" : {JsonUtility.ToJson(_lonLat)}" +
                        $"}}";
                    StartCoroutine(GssPoster.SaveUserData(_gasURL, _userName, message));
                }
                else if (_requestMethod == MethodNames.RemoveData)
                {
                    string message = $"{{" +
                        $"\"areaId\" : {_areaId}, " +
                        $"\"vertexId\" : {_vertexId} " +
                        $"}}";
                    StartCoroutine(GssPoster.RemoveData(_gasURL, _userName, message));
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
            _localGssData.RefreshUserNames(datas);
            foreach(var userName in _localGssData._userNames)
            {
                Debug.Log(userName);
            }
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
            foreach(var t in _localGssData.GetNearLonLatDatas(Vector2.zero, null))
            {
                Debug.Log(t.Key);
                foreach (var j in t.Value)
                {
                    Debug.Log(j);
                }
            }
        }
    }
}


