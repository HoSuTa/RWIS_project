using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GssDbManageWrapper
{
    public class UserDataManager
    {
        public string _localPlayerName
        {
            get { return _localPlayerName; }
            private set { _localPlayerName = value; }
        }
        public HashSet<string> _userNames
        {
            get
            {
                return _userNames;
            }
            private set
            {
                _userNames = value;
            }
        }

        public UserDataManager(string localPlayerName)
        {
            _localPlayerName = localPlayerName;
            _userNames = new HashSet<string>();
        }

        public void AddUserName(string userName)
        {
            _userNames.Add(userName);
        }
        public void RefreshUserNames(string[] userNames)
        {
            _userNames.Clear();
            _userNames = new HashSet<string>(userNames);
        }
        public void RefreshUserNames(HashSet<string> userNames)
        {
            _userNames.Clear();
            _userNames = userNames;
        }
        public void RefreshUserNames(PayloadData[] datas)
        {
            _userNames.Clear();
            foreach (var data in datas)
            {
                _userNames.Add(data.userName);
            }
        }

    }
}

