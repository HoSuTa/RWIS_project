using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

public class LineData
{
    public UserData _userData;

    public GameObject _lineObject;
    private LineRenderer _lineRenderer;

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
    }

    ~LineData()
    {
        UnityEngine.Object.Destroy(_lineObject);
    }
    public void RefreshLine()
    {
        UnityEngine.Object.Destroy(_lineObject);
    }

}
