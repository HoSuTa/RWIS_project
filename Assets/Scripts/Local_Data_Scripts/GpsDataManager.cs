using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;
using Mapbox.Utils;
using Mapbox.Unity.Map;


[RequireComponent(typeof(AbstractMap))]

[RequireComponent(typeof(GssDbHub))]
[RequireComponent(typeof(LonLatGetter))]
[RequireComponent(typeof(AreaDataManager))]
[RequireComponent(typeof(UserDataManager))]

public class GpsDataManager : MonoBehaviour
{
    AbstractMap _abstractMap;

    GssDbHub _gssDbHub;
    LonLatGetter _lonLatGetter;
    AreaDataManager _areaDataManager;
    UserDataManager _userDataManager;

    [SerializeField]
    private float _saveInterval = 10.0f;
    [SerializeField]
    private float _distanceUntilUpdate = .2f;
    //Making y big to make the initial save always valid.
    private Vector3 _lastUnityPos = _outlierPos;
    private static Vector3 _outlierPos = new Vector3(0, -100f, 0);

    private void Awake()
    {
        if (_abstractMap == null) _abstractMap = GetComponent<AbstractMap>();

        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();
        if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();

        StartCoroutine(SaveGpsDataPeriodically());
    }

    private IEnumerator SaveGpsDataPeriodically()
    {

        while (true)
        {
            if (_lonLatGetter.CanGetLonLat())
            {
                var gpsData = new Vector2d(_lonLatGetter.Latitude, _lonLatGetter.Longitude);
                var gpsUnityPos = _abstractMap.GeoToWorldPosition(gpsData);

                //Updates when the user moved far enough
                if ((gpsUnityPos - _lastUnityPos).magnitude > _distanceUntilUpdate)
                {
                    //Update local datas with gss datas.
                    LocalDataUpdater.Update(_userDataManager, _areaDataManager, _gssDbHub);
                    while (_userDataManager.IsUpdating || _areaDataManager.IsUpdating)
                    {
                        Debug.Log($"<color=blue>[GpsDataManager]</color> " +
                            $"Updating before checking the point.");
                        yield return new WaitForSeconds(1.0f);
                    }


                    var isClosed = _areaDataManager.IsCurrentAreaClosed(_userDataManager.LocalPlayerName);
                    var updatingAreaId = _areaDataManager.GetCurrentAreaId(_userDataManager.LocalPlayerName);
                    var savingVertexId = _areaDataManager.GetNextVertexId(
                        _userDataManager.LocalPlayerName, updatingAreaId);

                    //
                    // ..Closed Line Check HERE...
                    // In some cases, re-upload whole datas.
                    //
                    //local datas can be changed through updating with gss datas.

                    //No closed line, just need to update lat position.
                    _lastUnityPos = gpsUnityPos;
                }
            }

            yield return new WaitForSeconds(_saveInterval);
        }
    }

    private void LineClosedUpdateDatas(List<MessageJson> datas)
    {
        //Upload the datas, and get all the data by feedback function.
        _gssDbHub.UpdateDatas(_userDataManager.LocalPlayerName, datas,
            _ => LocalDataUpdater.Update(_userDataManager, _areaDataManager, _gssDbHub););
        _lastUnityPos = _outlierPos;
    }

}
