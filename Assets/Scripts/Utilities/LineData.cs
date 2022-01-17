using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

public class LineData
{
    public UserData _userData;

    public GameObject _lineObject;
    private LineRenderer _lineRenderer;

    private GameObject _lastPosUserObj;

    public LineData(UserData userData, List<Vector3> positions)
    {
        _userData = userData;

        _lineObject = new GameObject();
        _lineObject.transform.name = userData._userName + " Current Line";

        _lineRenderer = _lineObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;


        Material material = new Material(Shader.Find("Diffuse"));
        material.color = userData._color;

        _lineRenderer.material = material;

        _lineRenderer.startWidth = 5f;
        _lineRenderer.endWidth = 5f;

        _lineRenderer.positionCount = positions.Count;
        _lineRenderer.SetPositions(positions.ToArray());

        _lastPosUserObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _lastPosUserObj.transform.position = positions[positions.Count - 1];
        _lastPosUserObj.transform.localScale *= 12;
        _lastPosUserObj.GetComponent<MeshRenderer>().material = material;
    }

    ~LineData()
    {
        UnityEngine.Object.Destroy(_lineObject);
        UnityEngine.Object.Destroy(_lastPosUserObj);
    }
    public void RefreshLine()
    {
        UnityEngine.Object.Destroy(_lineObject);
        UnityEngine.Object.Destroy(_lastPosUserObj);
    }

}
