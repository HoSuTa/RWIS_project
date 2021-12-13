using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var lineRenderer         = GetComponent< WorldLineRenderer>();
        var areaCalculator       = GetComponent<LineAreaCalculator>();
        areaCalculator.vertices  = lineRenderer.lonLatsVertices;
        var closedLine           = GetComponent<ClosedLine>();
        closedLine.vertices      = lineRenderer.lonLatsVertices;
    }

    // Update is called once per frame
    void Update()
    {
        var indices = GetComponent<ClosedLine>().GetClosedIndices();
        if (indices!=null&& indices.Count >= 3){
            GetComponent<LineAreaCalculator>().indices = indices;
            Debug.Log(GetComponent<LineAreaCalculator>().CalulateArea());
        }
    }
}
