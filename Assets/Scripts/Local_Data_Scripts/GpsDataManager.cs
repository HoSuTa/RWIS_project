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
    private Vector3 _lastUnityPos = new Vector3(0, -10000f, 0);

    private void Awake()
    {
        if (_abstractMap == null) _abstractMap = GetComponent<AbstractMap>();

        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();
        if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
    }

    private IEnumerator SaveGpsData()
    {
        while (true)
        {
            if (_lonLatGetter.CanGetLonLat())
            {
                var gpsData = new Vector2d(_lonLatGetter.Latitude, _lonLatGetter.Longitude);
                var gpsUnityPos = _abstractMap.GeoToWorldPosition(gpsData);
                
                //Updates when the user moved far enough
                if( (gpsUnityPos - _lastUnityPos).magnitude > _distanceUntilUpdate)
                {
                    var updatingAreaId = _areaDataManager.GetCurrentAreaId();
                    var savingVertexId = _areaDataManager.GetNextVertexId(
                        _userDataManager.GetPlayerName(), updatingAreaId);

                    _gssDbHub.SaveData(
                        _userDataManager.GetPlayerName(), updatingAreaId, savingVertexId, gpsUnityPos);
                    _lastUnityPos = gpsUnityPos;
                }
                
            }
            yield return new WaitForSeconds(_saveInterval);
        }
    }







}
