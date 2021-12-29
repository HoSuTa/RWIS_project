using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GssDbManageWrapper
{
    public class LocalDataHub
    {
        public Dictionary<string, List<MessageJson>> _allDatas;

        public LocalDataHub()
        {
            _allDatas = new Dictionary<string, List<MessageJson>>();
        }

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

       
        private void AddData(ref Dictionary<string, List<MessageJson>> dataList, string userName, MessageJson data)
        {
            if(dataList.ContainsKey(userName))
            {
                dataList[userName].Add(data);
            }
            else
            {
                var newData = new List<MessageJson>();
                newData.Add(data);
                dataList.Add(userName, newData);
            }
        }

        private void RefreshDatas(ref Dictionary<string, List<MessageJson>> dataList, PayloadData[] datas)
        {
            dataList.Clear();
            foreach (var (userName, messageJson) in from d in datas
                        let userName = d.userName
                        let messageJson = d.ExtractMessageJson()
                        select (userName, messageJson))
            {
                AddData(ref dataList, userName, messageJson);
            }
        }

        public void RefreshAllDatas(PayloadData[] datas)
        {
            RefreshDatas(ref _allDatas, datas);

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

        public Dictionary<string, List<MessageJson>> GetAllNearPositionDatas(
            Vector2 targetLonLat, Func<Vector3, Vector3, bool> nearConditionFunc = null)
        {
            return GetNearPositionDatas(_allDatas, targetLonLat, nearConditionFunc);
        }


        private bool isTwoPositionCloseEnough(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude < .4f;
        }
    }
}