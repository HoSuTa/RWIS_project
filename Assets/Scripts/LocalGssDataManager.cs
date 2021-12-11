using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GssDbManageWrapper
{
    public class LocalGssDataManager
    {
        public HashSet<string> _userNames = new HashSet<string>(); 
        public Dictionary<string, List<MessageJson>> _userDatas = new Dictionary<string, List<MessageJson>>();

        public List<Vector2> GetLonLatArrays(string userName, int areaId)
        {
            if (_userDatas.ContainsKey(userName))
            {
                var filteredUserDatas = _userDatas[userName];
                List<Vector2> lonLatArrays = new List<Vector2>();
                for (int i = 0; i < filteredUserDatas.Count; i++)
                {
                    lonLatArrays.Add(filteredUserDatas[i].lonLat);
                }
                return lonLatArrays;
            }
            else
            {
                return null;
            }
        }

        public List<Vector2> GetAllLonLatArrays()
        {
            List<Vector2> lonLatArrays = new List<Vector2>();
            foreach (List<MessageJson> list in _userDatas.Values)
            {
                foreach (MessageJson m in list)
                {
                    lonLatArrays.Add(m.lonLat);
                }
            }
            return lonLatArrays;
        }

        public void AddUserName(string userName)
        {
            _userNames.Add(userName);
        }
        public void AddUserData(string userName, MessageJson data)
        {
            if(_userDatas.ContainsKey(userName))
            {
                _userDatas[userName].Add(data);
            }
            else
            {
                var dataList = new List<MessageJson>();
                dataList.Add(data);
                _userDatas.Add(userName, dataList);
            }
        }
        public void RefreshUserNames(string[] userNames)
        {
            _userNames = new HashSet<string>(userNames);
        }
        public void RefreshUserNames(HashSet<string> userNames)
        {
            _userNames = userNames;
        }
        public void RefreshUserNames(PayloadData[] datas)
        {
            var userNames = new HashSet<string>();
            foreach (var data in datas)
            {
                userNames.Add(data.userName);
            }
            _userNames = userNames;
        }
        public void RefreshUserDatas(Dictionary<string, List<MessageJson>> userDatas)
        {
            _userDatas = userDatas;
        }
        public void RefreshUserDatas(PayloadData[] datas)
        {
            var userDatas = new Dictionary<string, List<MessageJson>>();
            for(int i = 0; i < datas.Length; i++)
            {
                var userName = datas[i].userName;
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);

                if (userDatas.ContainsKey(userName))
                {
                    userDatas[userName].Add(messageJson);
                }
                else
                {
                    var dataList = new List<MessageJson>();
                    dataList.Add(messageJson);
                    userDatas.Add(userName, dataList);
                }
            }
            _userDatas = userDatas;
        }
        public Dictionary<string, List<MessageJson>> GetNearLonLatDatas(Vector2 targetLonLat, Func<Vector2, Vector2, bool> nearConditionFunc)
        {
            if(nearConditionFunc == null)
            {
                nearConditionFunc = isTwoPositionCloseEnough;
            }

            Dictionary<string, List<MessageJson>> nearLonLatDatas = new Dictionary<string, List<MessageJson>>();
            foreach (var key in _userDatas.Keys)
            {
                nearLonLatDatas.Add(key, _userDatas[key].Where(v => nearConditionFunc(v.lonLat, targetLonLat)).ToList());
                if(nearLonLatDatas[key].Count == 0)
                {
                    nearLonLatDatas.Remove(key);
                }
            }
            return nearLonLatDatas;
        }

        private bool isTwoPositionCloseEnough(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude < .4f;
        }
    }
}