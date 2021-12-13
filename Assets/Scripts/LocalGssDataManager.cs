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

        public List<Vector2> GetUserPositions(string userName, int areaId)
        {
            if (_alldatas.ContainsKey(userName))
            {
                var filteredUserDatas = _alldatas[userName];
                List<Vector2> positions = new List<Vector2>();
                for (int i = 0; i < filteredUserDatas.Count; i++)
                {
                    positions.Add(filteredUserDatas[i].position);
                }
                return positions;
            }
            else
            {
                return null;
            }
        }

        public List<Vector2> GetAllPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (List<MessageJson> list in _alldatas.Values)
            {
                foreach (MessageJson m in list)
                {
                    positions.Add(m.position);
                }
            }
            return positions;
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
            return GetNearPositionDatas(_alldatas, targetLonLat, nearConditionFunc);
        }


        private bool isTwoPositionCloseEnough(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude < .4f;
        }
    }
}