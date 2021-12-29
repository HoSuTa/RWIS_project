using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;
using UnityEngine.UI;
using System.Linq;


[RequireComponent(typeof(GssDbHub))]
[RequireComponent(typeof(LocalDataHub))]
[RequireComponent(typeof(UserDataManager))]

public class GssLocalManager : MonoBehaviour
{
    GssDbHub _gssDbHub;
    LocalDataHub _localDataHub;
    UserDataManager _userDataManager;

    [Header("テスト用のパラメータ")]
    [SerializeField]
    private bool _updateRequest = false;
    [SerializeField]
    private string _playerName = "tester";
    [SerializeField]
    private Text _uiText;

    private void Awake()
    {
        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_localDataHub == null) _localDataHub = GetComponent<LocalDataHub>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
        _userDataManager._localPlayerName = _playerName;
    }

    private void Update()
    {
        if (_updateRequest)
        {
            UpdateLocalAllDatas();
            _updateRequest = false;
        }
    }

    public void UploadNewData(string userName, int areaId, int vertexId, Vector3 position)
    {
        _gssDbHub.SaveData(userName, areaId, vertexId, position, PostFeedback);
    }
    public void RemoveData(string userName, int areaId, int vertexId)
    {
        _gssDbHub.RemoveData(userName, areaId, vertexId, PostFeedback);
    }


    private void PostFeedback(string response)
    {
        UpdateLocalAllDatas();
        UpdateLocalUserNames();
    }





    private void UpdateLocalAllDatas()
    {
        _gssDbHub.GetAllDatas(LocalAllDatasSetFeedBack);
    }

    private void UpdateLocalUserNames()
    {
        _gssDbHub.GetUserNames(LocalUserNamesSetFeedBack);
    }

    private void LocalAllDatasSetFeedBack(PayloadData[] datas)
    {
        _localDataHub.RefreshAllDatas(datas);
        TempUIVisualizeAllDatas();
    }
    private void LocalUserNamesSetFeedBack(PayloadData[] datas)
    {
        _userDataManager.RefreshUserNames(datas);
        TempUIVisualizeAllNames();
    }




    private void TempUIVisualizeAllDatas()
    {
        var datas = _localDataHub.GetAllDatas();
        _uiText.text = "";
        for (int i = 0; i < datas.Count; i++)
        {
            _uiText.text = string.Concat(_uiText.text, $"Data[{i}]:\n");
            _uiText.text = string.Concat(_uiText.text, $"areaId: {datas[i].areaId} ");
            _uiText.text = string.Concat(_uiText.text, $"vertexId: {datas[i].vertexId}\n");
            _uiText.text = string.Concat(_uiText.text, $"position: {datas[i].position.ToString()}\n");
        }
    }
    private void TempUIVisualizeAllNames()
    {
        var datas = _userDataManager.GetUserNames().ToArray();
        _uiText.text = "";
        for (int i = 0; i < datas.Length; i++)
        {
            _uiText.text = string.Concat(_uiText.text, $"Data[{i}]:\n");
            _uiText.text = string.Concat(_uiText.text, $"userName: {datas[i]}\n");
        }
    }
}
