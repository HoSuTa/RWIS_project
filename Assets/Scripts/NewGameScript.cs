using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;

public class NewGameScript : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private double[] initialLonLats;
    [SerializeField]
    private int initialZoom;
    [SerializeField]
    private double minLength;
    [SerializeField]
    private List<Vector2> lonLatsVertices;

    [SerializeField]
    AbstractMap _map;

    ILocationProvider _locationProvider;
    [SerializeField] private LonLatGetter lonLatGetter;


    // Start is called before the first frame update
    void Start()
    {

        /*
        var abstractMap = GetComponent<AbstractMap>();
        abstractMap.Initialize(new Vector2d(initialLonLats[0], initialLonLats[1]), initialZoom);
        */
        while (true)
        {
            //if (lonLatGetter.CanGetLonLat())
            //{
                //_map.Initialize(new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude), _map.AbsoluteZoom);
                //break;
            //}
           //yield return new WaitForSeconds(1.0f);

        }

        var lineRenderer = GetComponent<WorldLineRenderer>();
        lineRenderer.lonLatsVertices = lonLatsVertices;
        var areaCalculator = GetComponent<LineAreaCalculator>();
        areaCalculator.vertices = lonLatsVertices;
        var closedLine = GetComponent<ClosedLine>();
        closedLine.vertices = lonLatsVertices;
        closedLine.minLength = minLength;
    }

    // Update is called once per frame
    void Update()
    {
        var indices = GetComponent<ClosedLine>().GetClosedIndices();
        if (indices != null && indices.Count >= 3)
        {
            GetComponent<LineAreaCalculator>().indices = indices;
            text.text = $"Score: {GetComponent<LineAreaCalculator>().CalulateArea()}";
        }
    }
}

