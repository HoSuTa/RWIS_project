using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
public class WorldLineRenderer : MonoBehaviour
{
    [SerializeField]
    private List<Vector2> lonLatsVertices;
    [SerializeField]
    private AbstractMap    mapBoxMap;
    private GameObject     gameObject;
    [SerializeField]
    private LineRenderer   renderer;
    // Start is called before the first frame update
    void Start()
    {
        gameObject= new GameObject();
        mapBoxMap = GetComponent<AbstractMap>();
        renderer  = gameObject.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lonLatsVertices.Count == 0){
            return;
        }
        renderer.positionCount = lonLatsVertices.Count;
        for(var i=0;i<lonLatsVertices.Count;++i)
        {
            var worldPos = mapBoxMap.GeoToWorldPosition(new Vector2d(lonLatsVertices[i].x,lonLatsVertices[i].y));
            worldPos[1] += 10.0f;
            renderer.SetPosition(i,worldPos);
        }
    }
}
