using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
/// <summary>範囲管理クラス</summary>
public class RangeContainer : MonoBehaviour
{
    
    private const float minDistance = 0.1f; 
    public Vector2 [] points ;
    public void    AddPoint(Vector2 point)
    {
        var curLength = points.Length;
        System.Array.Resize<Vector2>(ref points,points.Length+1);
        points[curLength] = point;
    }

    public Vector2 PopPoint() 
    {
        
        var endPoint = points[points.Length-1];
        System.Array.Resize<Vector2>(ref points,points.Length-1);
        return endPoint;
    }

    public bool    IsClosed()
    {
        if (points.Length==0){
            return false;
        }
        var begPoint = points[0];
        var endPoint = points[points.Length-1];
        if (Vector2.Distance(begPoint,endPoint)>minDistance){
            return false;
        }
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
