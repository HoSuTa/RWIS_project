using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GssDbManageWrapper
{
    public class UserData
    {
        public string _userName;
        public Color _color;

        public UserData(string userName, Color color)
        {
            _userName = userName;
            _color = color;
        }
    }

    public class UserDataManager : MonoBehaviour
    {
        [SerializeField]
        private string _localPlayerName = "";
        public string LocalPlayerName
        {
            get => _localPlayerName;
            set => _localPlayerName = value;
        }

        private bool _isUpdating = false;

        public HashSet<string> _userNames = new HashSet<string>();
        public HashSet<UserData> _userDatas = new HashSet<UserData>();


        private void Start()
        {
            if (_localPlayerName == "")
            {
                Debug.Log($"<color=red>[UserDataManager]</color> " +
                    $"{nameof(_localPlayerName)} is \"{_localPlayerName}\".");
            }
        }

        public Color RandomColor()
        {
            bool colorExists = true;
            Color randomColor = Color.white;
            while (colorExists)
            {
                randomColor = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
                foreach (var u in _userDatas)
                {
                    if (u._color == randomColor)
                    {
                        colorExists = false;
                    }
                }
            }

            return randomColor;
        }

        public void AddUserData(UserData userData)
        {
            _userDatas.Add(userData);
        }

        public void AddUserName(string userName)
        {
            _userNames.Add(userName);
        }

        public void RefreshUserNames(PayloadData[] datas)
        {
            ClearUserNames();
            foreach (var d in datas)
            {
                AddUserName(d.userName);
            }
        }
        public void RefreshUserDatas(PayloadData[] datas)
        {
            ClearUserNames();
            foreach (var d in datas)
            {
                AddUserData(new UserData(d.userName, RandomColor()));
            }
        }

        public void UpdateAllUserNamesToGss(GssDbHub gssDbHub)
        {
            _isUpdating = true;
            gssDbHub.GetUserNames(GetUserNamesFeedBack);
        }
        private void GetUserNamesFeedBack(PayloadData[] datas)
        {
            RefreshUserNames(datas);
            RefreshUserDatas(datas);
            _isUpdating = false;
        }

        public void ClearUserNames()
        {
            _userNames.Clear();
        }

        public bool IsUpdating
        {
            get { return _isUpdating; }
        }

    }
}

