using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GssDbManageWrapper
{
    public class UserDataManager : MonoBehaviour
    {
        [SerializeField]
        private string _localPlayerName = "";
        public string LocalPlayerName
        {
            get => _localPlayerName;
            set => _localPlayerName = value;
        }
        private HashSet<string> _userNames = new HashSet<string>();
        public HashSet<string> UserNames
        {
            get => _userNames;
            set => _userNames = value;  
        }

        private void Awake()
        {
            if (_localPlayerName == null || _localPlayerName == "")
            {
                Debug.Log($"<color=red>[UserDataManager]</color> " +
                    $"{nameof(_localPlayerName)} is {_localPlayerName}.");
            }
        }

        public void AddUserName(string userName)
        {
            _userNames.Add(userName);
        }
        public void RefreshUserNames(string[] userNames)
        {
            ClearUserNames();
            _userNames = new HashSet<string>(userNames);
        }
        public void RefreshUserNames(HashSet<string> userNames)
        {
            ClearUserNames();
            _userNames = userNames;
        }
        public void RefreshUserNames(PayloadData[] datas)
        {
            ClearUserNames();
            foreach (var d in datas)
            {
                AddUserName(d.userName);
            }
        }

        public void UpdateAllUserNamesToGss(GssDbHub gssDbHub)
        {
            gssDbHub.GetUserNames(GetUserNamesFeedBack);
        }
        private void GetUserNamesFeedBack(PayloadData[] datas)
        {
            RefreshUserNames(datas);
        }

        public void ClearUserNames()
        {
            _userNames.Clear();
        }


    }
}

