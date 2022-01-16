using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GssDbManageWrapper;
public class PolyLineData
{
    public UserData _userData;
    public int _areaId;
    public float _score;
    private Transform _canvasTransform;
    public GameObject _scoreTextObject;
    private Text      _scoreText;
    public GameObject _lineObject;
    private LineRenderer _lineRenderer;

    public PolyLineData(UserData userData, int areaId, List<Vector3> positions)
    {
        float score = Mathf.Abs(CalcSignedArea(positions));
        _userData = userData;
        _areaId = areaId;
        _score  = score;
        _canvasTransform = GameObject.Find("Canvas").transform;

        _scoreTextObject = new GameObject();
        _scoreTextObject.transform.SetParent(_canvasTransform);
        _scoreTextObject.transform.localScale = new Vector3(1, 1, 1);
        _scoreText = _scoreTextObject.AddComponent<Text>();
        _scoreText.text = "Score: " + score.ToString();
        _scoreText.font = Resources.GetBuiltinResource (typeof(Font), "Arial.ttf") as Font;
        _scoreText.fontSize = 14;
        _scoreText.color = Color.black;
        var centroid  = CalcCentroid(positions);
        var screenPos = Camera.main.WorldToScreenPoint(centroid);
        screenPos.z   = 1.0f;
        _scoreText.rectTransform.position   = screenPos;
        _scoreText.rectTransform.sizeDelta = new Vector2(100.0f,30.0f);
        _lineObject = new GameObject();
        _lineRenderer = _lineObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.startColor = _userData._color;
        _lineRenderer.endColor   = _userData._color;
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
}
