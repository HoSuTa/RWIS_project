using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GssDbManageWrapper
{
    public class LocalGssDataManager
    {
        public HashSet<string> _userNames = new HashSet<string>(); 
        public Dictionary<string, MessageJson> _userDatas = new Dictionary<string, MessageJson>();

        public List<Vector2> GetLonLatArrays(string userName, int areaId)
        {
            var filteredUserDatas = _userDatas.Where(i => i.Key == userName && i.Value.areaId == areaId).ToArray();
            List<Vector2> lonLatArrays = new List<Vector2>();
            for (int i = 0; i < filteredUserDatas.Length; i++)
            {
                lonLatArrays.Add(filteredUserDatas[i].Value.lonLat);
            }
            return lonLatArrays;
        }

        public void AddUserName(string userName)
        {
            _userNames.Add(userName);
        }
        public void AddUserData(string userName, MessageJson data)
        {
            _userDatas.Add(userName, data);
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
        public void RefreshUserDatas(Dictionary<string, MessageJson> userDatas)
        {
            _userDatas = userDatas;
        }
        public void RefreshUserDatas(PayloadData[] datas)
        {
            var userDatas = new Dictionary<string, MessageJson>();
            for(int i = 0; i < datas.Length; i++)
            {
                var userName = datas[i].userName;
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);
                userDatas.Add(userName, messageJson);
            }
            _userDatas = userDatas;
        }
        public Dictionary<string, MessageJson> searchNearLonLat(Vector2 targetLonLat, Func<Vector2, Vector2, bool> nearConditionFunc)
        {
            if(nearConditionFunc == null)
            {
                nearConditionFunc = isTwoPositionCloseEnough;
            }
            return _userDatas.Where(
                i => nearConditionFunc(i.Value.lonLat, targetLonLat) 
                ).ToDictionary(i => i.Key, i => i.Value);
        }

        private bool isTwoPositionCloseEnough(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude < .4f;
        }
    }
}