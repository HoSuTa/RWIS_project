using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
public static class MapboxExtensions
{
    public static List<Vector3> GeoToWorldPositions(this AbstractMap abstractMap, List<Vector2> lonLatVertices)
    {
        List<Vector3> worldPositions = new List<Vector3>();
        for(var i=0;i<lonLatVertices.Count;++i)
        {
            var worldPos = abstractMap.GeoToWorldPosition(new Vector2d(lonLatVertices[i].x,lonLatVertices[i].y));
            worldPositions.Add(worldPos);
        }
        return worldPositions;
    }
};
