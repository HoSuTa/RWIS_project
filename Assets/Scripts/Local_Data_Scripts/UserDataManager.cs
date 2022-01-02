﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GssDbManageWrapper
{
    public class UserDataManager : MonoBehaviour
    {
        public string _localPlayerName = "";
        public HashSet<string> _userNames = new HashSet<string>();

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

        public void ClearUserNames()
        {
            _userNames.Clear();
        }

        public HashSet<string> GetUserNames()
        {
            return _userNames;
        }
    }
}
