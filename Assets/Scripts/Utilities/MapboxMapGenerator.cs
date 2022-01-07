using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

[RequireComponent(typeof(AbstractMap))]
[RequireComponent(typeof(LonLatGetter))]
public class MapboxMapGenerator : MonoBehaviour
{
    AbstractMap _abstractMap;
    LonLatGetter _lonLatGetter;

    [SerializeField] public float _zoomSize = 16.5f;

    private void Awake()
    {
        if (_abstractMap == null) _abstractMap = GetComponent<AbstractMap>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();

        _abstractMap.InitializeOnStart = false;
        StartCoroutine(GenerateMapboxMap());
    }

    private IEnumerator GenerateMapboxMap()
    {
        while (true)
        {
            if (_lonLatGetter.CanGetLonLat())
            {
                _abstractMap.ResetMap();
                var mapCenterLonLat = new Vector2d(
                    _lonLatGetter.Latitude, _lonLatGetter.Longitude);
                _abstractMap.SetZoom(_zoomSize);
                _abstractMap.Initialize(mapCenterLonLat, _abstractMap.AbsoluteZoom);
                break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
