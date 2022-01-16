using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PolyLineData
{
    [SerializeField]
    private string     _userName;
    [SerializeField]
    private Color      _color;
    [SerializeField]
    private int        _areaId;
    [SerializeField]
    private float      _score;
    private Transform  _canvasTransform;
    private GameObject _lineObject;
    private GameObject _textObject;
    // Start is called before the first frame update
    public PolyLineData()
    {
        _canvasTransform = GameObject.Find("Canvas").transform;
        _textObject      = new GameObject();
        _textObject.transform.SetParent(_canvasTransform);
        _textObject.transform.localScale = new Vector3(1,1,1);
        if (!UserColorMap.ContainsKey(userName))
        {
            UserColorMap[userName] = RandomColor();
        }
        _color = UserColorMap[userName];
    }
    void OnRemove()
    {
        UnityEngine.Object.Destroy(_textObject);
        UnityEngine.Object.Destroy(_lineObject);
    }

}
