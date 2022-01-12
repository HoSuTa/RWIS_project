using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

[RequireComponent(typeof(AbstractMap))]
[RequireComponent(typeof(LonLatGetter))]
public class MapboxMapManager : MonoBehaviour
{
    AbstractMap _abstractMap;
    LonLatGetter _lonLatGetter;

    [SerializeField] public Vector2 _lonLatFromPlayerSetting;
    [SerializeField] public float _zoomSize = 16.5f;

    private void Awake()
    {
        if (_abstractMap == null) _abstractMap = GetComponent<AbstractMap>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();

        _abstractMap.InitializeOnStart = false;
    }

    private void Start()
    {
        RegenerateMapboxTexture(_lonLatFromPlayerSetting);
    }

    private void RegenerateMapboxTexture(Vector2 lonLat)
    {
        _abstractMap.ResetMap();
        var mapCenterLonLat = new Vector2d(lonLat.x, lonLat.y);
        _abstractMap.SetZoom(_zoomSize);
        _abstractMap.Initialize(mapCenterLonLat, _abstractMap.AbsoluteZoom);
    }
}
