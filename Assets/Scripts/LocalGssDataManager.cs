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
        public Dictionary<string, List<MessageJson>> _alldatas = new Dictionary<string, List<MessageJson>>();

        public List<Vector2> GetLonLatArrays(string userName, int areaId)
        {
            if (_alldatas.ContainsKey(userName))
            {
                var filteredUserDatas = _alldatas[userName];
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
            foreach (List<MessageJson> list in _alldatas.Values)
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
        public void AddOneData(string userName, MessageJson data)
        {
            if(_alldatas.ContainsKey(userName))
            {
                _alldatas[userName].Add(data);
            }
            else
            {
                var dataList = new List<MessageJson>();
                dataList.Add(data);
                _alldatas.Add(userName, dataList);
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
        private Dictionary<string, List<MessageJson>> RefreshDatas(PayloadData[] datas)
        {
            var newDatas = new Dictionary<string, List<MessageJson>>();
            for (int i = 0; i < datas.Length; i++)
            {
                var userName = datas[i].userName;
                var messageJson = JsonUtility.FromJson<MessageJson>(datas[i].message);

                if (newDatas.ContainsKey(userName))
                {
                    newDatas[userName].Add(messageJson);
                }
                else
                {
                    var dataList = new List<MessageJson>();
                    dataList.Add(messageJson);
                    newDatas.Add(userName, dataList);
                }
            }
            return newDatas;
        }
        public void RefreshUserDatas(PayloadData[] datas)
        {
            _userDatas = RefreshDatas(datas);
        }
        public void RefreshAllDatas(PayloadData[] datas)
        {
            _alldatas = RefreshDatas(datas);
        }
        private Dictionary<string, List<MessageJson>> GetNearLonLatDatas(
             Dictionary<string, List<MessageJson>>  searchingDatas, Vector2 targetLonLat, Func<Vector2, Vector2, bool> nearConditionFunc)
        {
            if (nearConditionFunc == null)
            {
                nearConditionFunc = isTwoPositionCloseEnough;
            }

            Dictionary<string, List<MessageJson>> nearLonLatDatas = new Dictionary<string, List<MessageJson>>();
            foreach (var key in searchingDatas.Keys)
            {
                nearLonLatDatas.Add(key, searchingDatas[key].Where(v => nearConditionFunc(v.lonLat, targetLonLat)).ToList());
                if (nearLonLatDatas[key].Count == 0)
                {
                    nearLonLatDatas.Remove(key);
                }
            }
            return nearLonLatDatas;
        }
        public Dictionary<string, List<MessageJson>> GetNearLonLatDatasInUser(
            Vector2 targetLonLat, Func<Vector2, Vector2, bool> nearConditionFunc)
        {
            return GetNearLonLatDatas(_userDatas, targetLonLat, nearConditionFunc);
        }

        public Dictionary<string, List<MessageJson>> GetNearLonLatDatasInAll(
            Vector2 targetLonLat, Func<Vector2, Vector2, bool> nearConditionFunc)
        {
            return GetNearLonLatDatas(_alldatas, targetLonLat, nearConditionFunc);
        }


        private bool isTwoPositionCloseEnough(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude < .4f;
        }
    }
}