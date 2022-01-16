﻿using System.Collections;
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
    private static Vector3 _outlierPos = new Vector3(0, 0, -100);

    private void Awake()
    {
        if (_abstractMap == null) _abstractMap = GetComponent<AbstractMap>();

        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();
        if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();

        StartCoroutine(SaveGpsDataPeriodically());
    }



    // Start is called before the first frame update
    public Dictionary<string, HashSet<int>> GetAllAreaIdMap()
    {
        var allDatas = _areaDataManager._allDatas;
        var areaIdMap = new Dictionary<string, HashSet<int>>();
        foreach (var pair in allDatas)
        {
            if (!areaIdMap.ContainsKey(pair.Key))
            {
                areaIdMap[pair.Key] = new HashSet<int>();
            }
            foreach (var data in pair.Value)
            {
                areaIdMap[pair.Key].Add(data.areaId);
            }
        }
        return areaIdMap;
    }
    public Dictionary<string, List<(int, List<Vector3>)>> GetAllPolygonPositions()
    {
        var allAreaIdMap = GetAllAreaIdMap();
        var allPolygonVertices = new Dictionary<string, List<(int, List<Vector3>)>>();
        foreach (var pair in allAreaIdMap)
        {
            if (!allPolygonVertices.ContainsKey(pair.Key))
            {
                allPolygonVertices[pair.Key] = new List<(int, List<Vector3>)>();
            }
            foreach (var areaId in pair.Value)
            {
                var polygonVertices = _areaDataManager.GetAreaVerticies(pair.Key, areaId);
                allPolygonVertices[pair.Key].Add((areaId, polygonVertices));
            }
        }
        return allPolygonVertices;
    }
    //String _userName
    static bool IsClosedPoint(Vector3 x1, Vector3 x2, float r_epsilon = 10)
    {
        return Vector3.Distance(x1, x2) < r_epsilon;
    }
    static bool IsClosedPointOnLines(Vector3 newPos, List<Vector3> linePositions, ref int crossIdx)
    {
        var isClosed = false;
        crossIdx = int.MaxValue;
        if (linePositions.Count > 2)
        {
            for (var i = 0; i < linePositions.Count - 2; ++i)
            {
                if (IsClosedPoint(newPos, linePositions[i]))
                {
                    isClosed = true;
                    crossIdx = i;
                    break;
                }
            }
        }
        return isClosed;
    }
    /*割とよさげ*/
    static bool IntersectLine(Vector3 origin, Vector3 target, Vector3 x1, Vector3 x2, ref float distance, float c_epsilon = 1e-5F)
    {
        Vector3 x3 = origin;
        Vector3 x4 = target;
        Vector3 x12 = x2 - x1;
        Vector3 x34 = x4 - x3;
        Vector3 x13 = x3 - x1;
        float det = x12.x * x34.y - x12.y * x34.x;
        float ps = x34.y * x13.x - x34.x * x13.y;
        float pt = x12.y * x13.x - x12.x * x13.y;
        float s = ps / det;
        float t = pt / det;
        if (c_epsilon < s && s < 1.0F - c_epsilon && c_epsilon < t && t < 1.0F - c_epsilon)
        {
            distance = t;
            return true;
        }
        return false;
    }
    static bool IntersectLines(Vector3 origin, Vector3 target, List<Vector3> linePositions, ref int crossIdx, ref float minDistance)
    {
        var isClosed = false;
        crossIdx = int.MaxValue;
        minDistance = float.MaxValue;
        if (linePositions.Count > 1)
        {
            for (var i = 0; i < linePositions.Count - 1; ++i)
            {
                float distance = 0.0f;
                if (IntersectLine(origin, target, linePositions[i], linePositions[i + 1], ref distance))
                {
                    if (distance < minDistance)
                    {
                        isClosed = true;
                        crossIdx = i + 1;
                        minDistance = distance;
                    }
                }
            }
        }
        return isClosed;
    }
    /*FIX->割とよさげ*/
    static bool ContainPoint(List<Vector3> points, Vector3 target, Vector3? normal = null, float r_epsilon = 1e-2F)
    {
        if (normal == null)
        {
            normal = new Vector3(0.0F, 0.0F, 1.0F);
        }
        double tht = 0.0F;
        for (var i = 0; i < points.Count; ++i)
        {
            var p_i = points[i];
            var p_i_1 = (i == points.Count - 1) ? points[0] : points[i + 1];
            var dp_i = p_i - target;
            var dp_i_1 = p_i_1 - target;
            var cosTht = Vector3.Dot(dp_i, dp_i_1) / (dp_i.magnitude * dp_i_1.magnitude);
            var angle = Mathf.Acos(cosTht);
            var cross = Vector3.Cross(dp_i, dp_i_1);
            if (Vector3.Dot(cross, (Vector3)normal) < 0.0F)
            {
                tht -= angle;
            }
            else
            {
                tht += angle;
            }
        }
        float val = Mathf.Abs((float)tht / (Mathf.PI * 2.0F));
        if (val < r_epsilon)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    static float CalcSignedArea(List<Vector3> points)
    {
        float signedArea = 0.0f;
        for (var i = 0; i < points.Count; ++i)
        {
            var p_i = points[i];
            var p_i_1 = (i == points.Count - 1) ? points[0] : points[i + 1];
            signedArea += (p_i.x * p_i_1.y - p_i.y * p_i_1.x);
        }
        return signedArea * 0.5F;
    }
    static Vector3 CalcCentroid(List<Vector3> points)
    {
        Vector3 centroid = new Vector3(0.0f, 0.0f);
        float area = 0.0F;
        for (var i = 0; i < points.Count; ++i)
        {
            var p_i = points[i];
            var p_i_1 = (i == points.Count - 1) ? points[0] : points[i + 1];
            var a_i = (p_i.x * p_i_1.y - p_i.y * p_i_1.x);
            area += a_i;
            centroid += (p_i + p_i_1) * a_i;
        }
        area /= 2.0f;
        centroid /= (6.0f * area);
        return centroid;
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

                    List<Vector3> currentAreaVerticies = _areaDataManager.GetCurrentAreaDatasAsVector();


                    Vector3 newPos = new Vector3(gpsUnityPos.x, gpsUnityPos.z, gpsUnityPos.y);

                    RelatedToClosedLine(newPos);


                    _lastUnityPos = gpsUnityPos;
                }
            }

            yield return new WaitForSeconds(_saveInterval);
        }
    }

    private void RelatedToClosedLine(Vector3 newPos)
    {
        var linePositions = _areaDataManager.GetCurrentAreaDatasAsVector();
        var allPolygonPositions = GetAllPolygonPositions();
        do
        {
            bool isContained = false;
            foreach (var pair in allPolygonPositions)
            {
                foreach (var (areaId, polygonPositions) in pair.Value)
                {
                    if (ContainPoint(polygonPositions, newPos))
                    {
                        isContained = true;
                        break;
                    }
                }
            }
            if (isContained)
            {
                Debug.Log("Contained!");
                return;
            }
            if (linePositions.Count > 0)
            {
                bool isIntersected = false;
                foreach (var pair in allPolygonPositions)
                {
                    foreach (var (areaId, polygonPositions) in pair.Value)
                    {
                        var i = 0;
                        foreach (var x1 in polygonPositions)
                        {
                            Vector3 x2 = (i == polygonPositions.Count - 1) ? polygonPositions[0] : polygonPositions[i + 1];
                            var distance = float.MaxValue;
                            if (IntersectLine(linePositions[linePositions.Count - 1], newPos, x1, x2, ref distance))
                            {
                                isIntersected = true;
                                break;
                            }
                            ++i;
                        }
                    }
                }
                if (isIntersected)
                {
                    Debug.Log("Intersected!");
                    return;
                }
            }
        } while (false);
        var isClosed = false;
        var crossIdx = int.MaxValue;
        var minDistance = float.MaxValue;
        //ClosedPoint
        do
        {
            if (IsClosedPointOnLines(newPos, linePositions, ref crossIdx))
            {
                isClosed = true;
                minDistance = 0.0F;
                break;
            }
            if (linePositions.Count == 0)
            {
                break;
            }
            if (IntersectLines(linePositions[linePositions.Count - 1], newPos, linePositions, ref crossIdx, ref minDistance))
            {
                isClosed = true;
            }
        } while (false);
        if (isClosed)
        {
            List<Vector3> tPositions = new List<Vector3>();
            if (minDistance != 0.0F)
            {
                var t0 = (1.0F - minDistance) * linePositions[linePositions.Count - 1] + minDistance * newPos;
                tPositions.Add(t0);
            }
            for (var i = crossIdx; i < linePositions.Count; ++i)
            {
                tPositions.Add(linePositions[i]);
            }
            float signedArea = CalcSignedArea(tPositions);
            if (signedArea < 0.0F)
            {
                tPositions.Reverse();
            }

            /*Remove*/
            {
                var removeIndices = new List<(string, int)>();
                {
                    foreach (var pair in allPolygonPositions)
                    {
                        foreach (var (areaId, polygonPositions) in pair.Value)
                        {
                            bool isContained = true;
                            foreach (var pos in polygonPositions)
                            {
                                if (!ContainPoint(tPositions, pos))
                                {
                                    isContained = false;
                                    break;
                                }
                            }
                            if (isContained)
                            {
                                removeIndices.Add((pair.Key, areaId));
                            }
                        }
                    }
                }
                foreach (var (userName, areaId) in removeIndices)
                {
                    _gssDbHub.RemoveArea(userName, areaId);
                }
            }
            _areaDataManager.UpdateCurrentAreaDatas(_userDataManager.LocalPlayerName, tPositions);
            LineClosedUpdateDatas(_areaDataManager._userCurrentArea);
        }
        else
        {
            _areaDataManager.AddPositinToCurrentAreaDatas(_userDataManager.LocalPlayerName, newPos);
            Debug.Log("Added position: " + newPos);
        }
    }

    private void LineClosedUpdateDatas(List<MessageJson> datas)
    {
        //Upload the datas, and get all the data by feedback function.
        _gssDbHub.UpdateDatas(_userDataManager.LocalPlayerName, datas,
            _ => LocalDataUpdater.Update(_userDataManager, _areaDataManager, _gssDbHub));
        _areaDataManager.RefreshCurrentAreaDatas();
        _lastUnityPos = _outlierPos;
    }

}
