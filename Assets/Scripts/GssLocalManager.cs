using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;
using UnityEngine.UI;
using System.Linq;


[RequireComponent(typeof(GssDbHub))]

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
        if(_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if(_localDataHub == null) _localDataHub = new LocalDataHub();
        if(_userDataManager == null) _userDataManager = new UserDataManager(_playerName);
    }

    private void Update()
    {
        if(_updateRequest)
        {
            UpdateLocalUserNames();
            _updateRequest = false;
        }
    }

    private void TempUIVisualizeAllDatas ()
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
}
