using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GssDbManageWrapper
{
    public enum MethodNames
    {
        GetUserNames,
        GetUserDatas,
        GetAllDatas,
        SaveMessage,
        RemoveData,
        CheckIfGssUrlValid,
        CheckIfGasUrlValid,
    }

    public class GssDbHub : MonoBehaviour
    {
        
        private void DefaultGetFeedBack(PayloadData[] datas)
        {
            foreach(var d in datas) Debug.Log(d);
        }
        private void DefaultPostFeedBack(string response)
        {
            Debug.Log(response);
        }



        public void GetAllDatas(Action<PayloadData[]> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultGetFeedBack;
            StartCoroutine(
                GssGetter.GetAllDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), response => localDataFeedback((PayloadData[])response)));
        }

        public void GetUserNames(Action<PayloadData[]> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultGetFeedBack;
            StartCoroutine(
                GssGetter.GetUserNames(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), response => localDataFeedback((PayloadData[])response)));
        }

        public void GetUserDatas(string userNames, Action<PayloadData[]> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultGetFeedBack;
            StartCoroutine(
                GssGetter.GetUserDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userNames, response => localDataFeedback((PayloadData[])response)));
        }


        public void SaveData(string userName, int areaId, int vertexId, Vector3 position)
        {
            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId}, " +
                        $"\"position\" : {JsonUtility.ToJson(position)}" +
                        $"}}";
            StartCoroutine(
                GssPoster.SaveUserData(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, message, response => DefaultPostFeedBack((string)response)));
        }

        public void RemoveData(string userName, int areaId, int vertexId)
        {
            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId} " +
                        $"}}";
            StartCoroutine(
                GssPoster.RemoveData(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, message, response => DefaultPostFeedBack((string)response)));
        }






        public void CheckIfGssUrlValid
        (
            string gssUrl,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGssUrlValid
                (
                    GasUrlManager.GetUrl(),
                    gssUrl,
                    response => GssUrlValidFeedBack
                    (
                        (string)response,
                        saveKeyFeedBack,
                        updateKeyRelatedUiFeedBack,
                        validFeedback,
                        invalidFeedback
                    )
                )
            );
        }

        private void GssUrlValidFeedBack
        (
            string response,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Error"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
                validFeedback?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }


        public void CheckIfGasUrlValid
        (
            string gasUrl,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGasUrlValid
                (
                    gasUrl,
                    response => GasUrlValidFeedBack
                    (
                        (string)response,
                        saveKeyFeedBack,
                        updateKeyRelatedUiFeedBack,
                        validFeedback,
                        invalidFeedback
                    )
                )
            );
        }

        private void GasUrlValidFeedBack
        (
            string response,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Cannot"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
                validFeedback?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }
    }
}


