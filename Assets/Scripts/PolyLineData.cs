using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

public class PolyLineData
{
    public UserData _userData;
    public int _areaId;
    public float _score;

    private Transform _canvasTransform;
    public GameObject _scoreTextObject;

    public GameObject _lineObject;
    private LineRenderer _lineRenderer;

    public PolyLineData(UserData userData, int areaId, List<Vector3> positions, float score)
    {
        _userData = userData;
        _areaId = areaId;
        _score = score;

        _canvasTransform = GameObject.Find("Canvas").transform;

        _scoreTextObject = new GameObject();
        _scoreTextObject.transform.SetParent(_canvasTransform);
        _scoreTextObject.transform.localScale = new Vector3(1, 1, 1);

        _lineObject = new GameObject();
        _lineRenderer = _lineObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.startColor = _userData._color;
        _lineRenderer.endColor = _userData._color;
        _lineRenderer.SetPositions(positions.ToArray());

    }

    ~PolyLineData()
    {
        UnityEngine.Object.Destroy(_lineObject);
        UnityEngine.Object.Destroy(_scoreTextObject);
    }
    void OnRemove()
    {
        UnityEngine.Object.Destroy(_scoreTextObject);
        UnityEngine.Object.Destroy(_lineObject);
    }

}
