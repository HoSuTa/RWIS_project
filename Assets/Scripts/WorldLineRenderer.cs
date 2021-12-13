using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
public class WorldLineRenderer : MonoBehaviour
{
    [SerializeField]
    public List<Vector2> lonLatsVertices;
    [SerializeField]
    public List<Vector3> worldVertices;
    [SerializeField]
    private AbstractMap  mapBoxMap;
    private GameObject   gameObject;
    [SerializeField]
    private LineRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        gameObject = new GameObject();
        mapBoxMap  = GetComponent<AbstractMap>();
        renderer   = gameObject.AddComponent<LineRenderer>();
        renderer.useWorldSpace = false;
        gameObject.transform.position = new Vector3(0.0f,0.0f,-10.0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (lonLatsVertices.Count == 0){
            return;
        }
        renderer.positionCount = lonLatsVertices.Count;
        worldVertices = mapBoxMap.GeoToWorldPositions(lonLatsVertices);
        renderer.SetPositions(worldVertices.ToArray());
    }
}
