using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

[RequireComponent(typeof(UserDataManager))]
[RequireComponent(typeof(AreaDataManager))]
public class LineDataManager : MonoBehaviour
{
    AreaDataManager _areaDataManager;
    UserDataManager _userDataManager;

    private LineData _lineData = null;

    private void Awake()
    {
        if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
    }

    public void UpdateLineData()
    {
        var userAreaData = _areaDataManager._userCurrentArea;
        List<Vector3> positions = new List<Vector3>();
        foreach (var d in userAreaData)
        {
            positions.Add(d.position);
        }
        _lineData = new LineData(_userDataManager.GetUserData(_userDataManager.LocalPlayerName), userAreaData[0].areaId, positions);
    }
}
