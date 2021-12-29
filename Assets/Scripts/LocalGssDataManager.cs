using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GssDbManageWrapper
{
    public class LocalGssDataManager
    {
        public Dictionary<string, List<MessageJson>> _userDatas = new Dictionary<string, List<MessageJson>>();
        public Dictionary<string, List<MessageJson>> _allDatas = new Dictionary<string, List<MessageJson>>();

        public List<MessageJson> GetUserDatas(string userName)
        {
            if (_allDatas.ContainsKey(userName))
            {
                var filteredUserDatas = _allDatas[userName];
                return filteredUserDatas;
            }
            else
            {
                return null;
            }
        }

        public List<MessageJson> GetAllDatas()
        {
            List<MessageJson> datas = new List<MessageJson>();
            foreach (List<MessageJson> list in _allDatas.Values)
            {
                foreach (MessageJson m in list)
                {
                    datas.Add(m);
                }
            }
            return datas;
        }

       
        public void AddData(string userName, MessageJson data)
        {
            if(_allDatas.ContainsKey(userName))
            {
                _allDatas[userName].Add(data);
            }
            else
            {
                var dataList = new List<MessageJson>();
                dataList.Add(data);
                _allDatas.Add(userName, dataList);
            }
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
            _allDatas = RefreshDatas(datas);
        }
        private Dictionary<string, List<MessageJson>> GetNearPositionDatas(
             Dictionary<string, List<MessageJson>>  searchingDatas, 
             Vector3 targetPos, 
             Func<Vector3, Vector3, bool> nearConditionFunc = null)
        {
            if (nearConditionFunc == null)
            {
                nearConditionFunc = isTwoPositionCloseEnough;
            }

            Dictionary<string, List<MessageJson>> nearPositionDatas = new Dictionary<string, List<MessageJson>>();
            foreach (var key in searchingDatas.Keys)
            {
                nearPositionDatas.Add(key, searchingDatas[key].Where(v => nearConditionFunc(v.position, targetPos)).ToList());
                if (nearPositionDatas[key].Count == 0)
                {
                    nearPositionDatas.Remove(key);
                }
            }
            return nearPositionDatas;
        }
        public Dictionary<string, List<MessageJson>> GetUserNearPositionDatas(
            Vector2 targetLonLat, Func<Vector3, Vector3, bool> nearConditionFunc)
        {
            return GetNearPositionDatas(_userDatas, targetLonLat, nearConditionFunc);
        }

        public Dictionary<string, List<MessageJson>> GetAllNearPositionDatas(
            Vector2 targetLonLat, Func<Vector3, Vector3, bool> nearConditionFunc)
        {
            return GetNearPositionDatas(_allDatas, targetLonLat, nearConditionFunc);
        }


        private bool isTwoPositionCloseEnough(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude < .4f;
        }
    }
}