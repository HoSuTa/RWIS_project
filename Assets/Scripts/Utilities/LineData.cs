using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

public class LineData
{
    public UserData _userData;
    public int _areaId;

    public GameObject _lineObject;
    private LineRenderer _lineRenderer;

    public LineData(UserData userData, int areaId, List<Vector3> positions)
    {
        _userData = userData;
        _areaId = areaId;

        _lineObject = new GameObject();
        _lineRenderer = _lineObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.startColor = _userData._color;
        _lineRenderer.endColor = _userData._color;
        _lineRenderer.SetPositions(positions.ToArray());

    }

    ~LineData()
    {
        UnityEngine.Object.Destroy(_lineObject);
    }
    void OnRemove()
    {
        UnityEngine.Object.Destroy(_lineObject);
    }

}
