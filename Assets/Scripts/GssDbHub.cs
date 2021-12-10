using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private string _gasURLjsonPos = "gasUrl.json";
        [SerializeField]
        private string _userName = "tester";
        [SerializeField]
        private string _message = "tester Unity Post";
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
                if (_requestMethod == MethodNames.GetUserDatas)
                {
                    StartCoroutine(GssGetter.GetUserDatas(_gasURL, _userName));
                }
                else if (_requestMethod == MethodNames.GetUserNames)
                {
                    StartCoroutine(GssGetter.GetUserNames(_gasURL));
                }
                else if (_requestMethod == MethodNames.SaveUserData)
                {
                    StartCoroutine(GssPoster.SaveUserData(_gasURL, _userName, _message));
                }
                _sendRequest = false;
            }
        }
    }
}


