using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;

public class GameScript : MonoBehaviour
{
    [SerializeField]
    private Text      text;
    [SerializeField]
    private double [] initialLonLats;
    [SerializeField]
    private int       initialZoom;
    [SerializeField]
    private double     minLength;
    [SerializeField]
    private List<Vector2> lonLatsVertices;
    // Start is called before the first frame update
    void Start()
    {
        var abstractMap          = GetComponent<AbstractMap>();
        abstractMap.Initialize(new Vector2d(initialLonLats[0],initialLonLats[1]),initialZoom);

        var lineRenderer             = GetComponent< WorldLineRenderer>();
        lineRenderer.lonLatsVertices = lonLatsVertices;
        var areaCalculator           = GetComponent<LineAreaCalculator>();
        areaCalculator.vertices      = lonLatsVertices;
        var closedLine               = GetComponent<ClosedLine>();
        //closedLine.vertices          = lonLatsVertices;
        closedLine.minLength         = minLength;
    }

    // Update is called once per frame
    void Update()
    {
        var indices = GetComponent<ClosedLine>().GetClosedIndices();
        if (indices!=null&& indices.Count >= 3){
            GetComponent<LineAreaCalculator>().indices = indices;
            text.text = $"Score: {GetComponent<LineAreaCalculator>().CalulateArea()}";
        }
    }
}
