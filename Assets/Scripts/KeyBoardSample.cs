using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KeyBoardSample: MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject  mainCamera;
    GameObject          canvas;
    List<Vector2>       linePositions;
    GameObject          lineObject;
    List<List<Vector2>> polyLinePositions;
    List<GameObject>    polyLineObjects;

    static bool    IsClosedPoint(Vector2 x1, Vector2 x2, float r_epsilon = 10)
    {
        return Vector2.Distance(x1,x2) < r_epsilon;
    }
    /*割とよさげ*/
    static bool    IntersectLine(Vector2 origin, Vector2 target, Vector2 x1, Vector2 x2, ref float distance, float c_epsilon = 1e-5F)
    {
        Vector2 x3  = origin;
        Vector2 x4  = target;
        Vector2 x12 = x2 - x1;
        Vector2 x34 = x4 - x3;
        Vector2 x13 = x3 - x1;
        float det = x12.x * x34.y - x12.y * x34.x;
        float ps  = x34.y * x13.x - x34.x * x13.y;
        float pt  = x12.y * x13.x - x12.x * x13.y;
        float s   = ps / det;
        float t   = pt / det;
        if (c_epsilon < s && s < 1.0F - c_epsilon && c_epsilon < t && t < 1.0F - c_epsilon)
        {
            distance = t;
            return true;
        }
        return false;
    }
    /*FIX->割とよさげ*/
    static bool    ContainPoint(ref List<Vector2> points, Vector2 target, Vector3? normal = null, float r_epsilon = 1e-2F)
    {
        if (normal==null)
        {
            normal = new Vector3(0.0F,0.0F,1.0F);
        }
        double tht = 0.0F;
        for (var i = 0; i < points.Count; ++i) {
            var p_i    = points[i];
            var p_i_1  = (i == points.Count - 1) ? points[0] : points[i + 1];
            var dp_i   = p_i   - target;
            var dp_i_1 = p_i_1 - target;
            var cosTht = Vector2.Dot(dp_i,dp_i_1) / (dp_i.magnitude  * dp_i_1.magnitude );
            var angle  = Mathf.Acos(cosTht);
            var cross  = Vector3.Cross(dp_i,dp_i_1);
            if (Vector3.Dot(cross,(Vector3)normal) < 0.0F) {
                tht -= angle;
            }
            else {
                tht += angle;
            }
        }
        float val = Mathf.Abs((float)tht/(Mathf.PI*2.0F));
        if (val < r_epsilon ){
            return false;
        }
        else {
            return true;
        }
    }
    static float   CalcSignedArea(ref List<Vector2> points) 
    {
        float signedArea = 0.0f;
        for (var i = 0; i < points.Count; ++i) {
            var p_i   = points[i];
            var p_i_1 = (i == points.Count - 1) ? points[0] : points[i + 1];
            signedArea += (p_i.x * p_i_1.y - p_i.y * p_i_1.x);
        }
        return signedArea * 0.5F;
    }
    static Vector2 CalcCentroid(ref List<Vector2> points)
    {
        Vector2 centroid = new Vector2(0.0f,0.0f);
        float area = 0.0F;
        for(var i = 0;i<points.Count;++i)
        {
            var p_i   = points[i];
            var p_i_1 = (i == points.Count - 1) ? points[0] : points[i + 1];
            var a_i   = (p_i.x * p_i_1.y - p_i.y * p_i_1.x);
            area     += a_i;
            centroid += (p_i+p_i_1) * a_i;
        }
        area     /=  2.0f;
        centroid /= (6.0f*area);
        return centroid;
    }
    void ClearOnClick()
    {
        Debug.Log("Clear!");
        var lineRenderer = lineObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        linePositions.Clear();
        foreach (var polyObject in polyLineObjects)
        {
            Destroy(polyObject);
        }
        polyLineObjects.Clear();
        polyLinePositions.Clear();
    }
    void Start()
    {
        linePositions     = new List<Vector2>();
        lineObject        = new GameObject();
        polyLinePositions = new List<List<Vector2>>();
        polyLineObjects       = new List<GameObject>();
        if (lineObject!=null)
        {
            lineObject.name            = "Line";
            var lineRenderer           = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material      = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor    = Color.black;
            lineRenderer.endColor      = Color.black;
            lineRenderer.startWidth    = 0.0125F;
            lineRenderer.endWidth      = 0.0125F;
            lineRenderer.positionCount = 0;
        }
        GameObject clearButton = transform.Find("Clear").gameObject;
        if (clearButton!=null)
        {
            clearButton.GetComponent<Button>().onClick.AddListener(ClearOnClick);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Key Down! "+Input.mousePosition.ToString());
            var newPos      = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            do{
                bool isContained = false;
                for (var i = 0; i< polyLinePositions.Count; ++i)
                {
                    var polyLinePosition = polyLinePositions[i];
                    if (ContainPoint(ref polyLinePosition,newPos))
                    {
                        isContained = true;
                        break;
                    }
                }
                if (isContained)
                {
                    Debug.Log("Contained!");
                    return;
                }
                if (linePositions.Count > 0)
                {
                    bool isIntersected = false;
                    foreach (var pol in polyLinePositions){
                        var i = 0;
                        foreach (var x1 in pol)
                        {
                            Vector2 x2 = (i == pol.Count-1) ? pol[0]: pol[i+1];
                            var distance  = float.MaxValue;
                            if (IntersectLine(linePositions[linePositions.Count-1], newPos, x1, x2, ref distance))
                            {
                                isIntersected = true;
                                break;
                            }
                            ++i;
                        }
                    }
                    if (isIntersected) 
                    {
                        Debug.Log("Intersected!");
                        return;
                    }
                }
            }while(false);
            var isClosed    = false;
            var crossIdx    = int.MaxValue;
            var minDistance = float.MaxValue;
            //ClosedPoint
            do {
                if (linePositions.Count > 2){
                    for (var i = 0 ; i < linePositions.Count-2; ++i )
                    {
                        if (IsClosedPoint(newPos, linePositions[i]))
                        {
                            isClosed    = true;
                            crossIdx    = i;
                            minDistance = 0.0f;
                            break;
                        }
                    }
                }
                if (isClosed)
                {
                    break;
                }
                if (linePositions.Count > 1)
                {
                    for (var i = 0 ; i < linePositions.Count-1; ++i )
                    {
                        float distance = 0.0f;
                        if (IntersectLine(linePositions[linePositions.Count-1],newPos,linePositions[i],linePositions[i+1],ref distance))
                        {
                            if (distance < minDistance)
                            {
                                isClosed    = true;
                                crossIdx    = i+1;
                                minDistance = distance;
                            }
                        }
                    }
                }
            }while(false);
            if (isClosed)
            {
                List<Vector2> tPositions = new List<Vector2>();
                if (minDistance != 0.0F)
                {
                    var t0 = (1.0F-minDistance)*linePositions[linePositions.Count-1]+minDistance*newPos;
                    tPositions.Add(t0);
                }
                for (var i = crossIdx; i < linePositions.Count; ++i)
                {
                    tPositions.Add(linePositions[i]);
                }
                float signedArea = CalcSignedArea(ref tPositions);
                if (signedArea < 0.0F)
                {
                    tPositions.Reverse();
                }
                /*Remove*/
                {
				    List<int> removeIndices = new List<int>();
                    {
                        var i = 0;
                        foreach (var poly in polyLinePositions)
                        {
                            bool isContained  = true;
                            foreach (var pos in poly) {
                                if (!ContainPoint(ref tPositions, pos)) {
                                    isContained = false;
                                    break;
                                }
                            }
                            if (isContained) {
                                removeIndices.Add(i);
                            }
                            ++i;
                        }
                    }
                    if (removeIndices.Count>0)
                    {
                        var offset = 0;
                        foreach (var idx in removeIndices) {
                            polyLinePositions.RemoveAt(idx - offset);
                            var polyObject = polyLineObjects[idx - offset];
                            polyLineObjects.RemoveAt(idx-offset);
                            Destroy(polyObject);
                            ++offset;
                        }
                        var i = 0;
                        foreach (var polyObject in polyLineObjects)
                        {
                            polyObject.name = "Polygon"+(i).ToString();
                            ++i;
                        }
                    }
                }
                Vector3[] newLinePositions = new Vector3[tPositions.Count+1];
                for (var i = 0; i< tPositions.Count; ++i)
                {
                    newLinePositions[i] = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(tPositions[i].x,tPositions[i].y,10.0f));
                }
                newLinePositions[tPositions.Count] = newLinePositions[0];
                {
                    polyLineObjects.Add(new GameObject());
                    polyLinePositions.Add(tPositions);
                    polyLineObjects[polyLineObjects.Count-1].name = "Polygon"+(polyLineObjects.Count-1).ToString();
                    var area                   = Mathf.Abs(CalcSignedArea(ref tPositions));
                    var centroid               = CalcCentroid(ref tPositions);
                    var text                   = polyLineObjects[polyLineObjects.Count-1].AddComponent<Text>();
                    text.text                  = "Area: " + area.ToString();
                    text.rectTransform.anchoredPosition = centroid;
                    // text.transform.Translate(mainCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(centroid.x,centroid.y,10.0F)));
                    var lineRenderer           = polyLineObjects[polyLineObjects.Count-1].AddComponent<LineRenderer>();
                    lineRenderer.positionCount = tPositions.Count+1;
                    lineRenderer.SetPositions(newLinePositions);
                    lineRenderer.material      = new Material(Shader.Find("Sprites/Default"));
                    Color polyColor            = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),1.0f);
                    lineRenderer.startColor    = polyColor;
                    lineRenderer.endColor      = polyColor;
                    lineRenderer.startWidth    = 0.0125F;
                    lineRenderer.endWidth      = 0.0125F;
                }
                {
                    lineObject.GetComponent<LineRenderer>().positionCount = 0;
                    linePositions.Clear();
                }
            }
            else{
                var lineRenderer = lineObject.GetComponent<LineRenderer>();
                linePositions.Add(newPos);
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(linePositions.Count-1, mainCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(linePositions[linePositions.Count-1].x,linePositions[linePositions.Count-1].y,10.0f)));
            }
        }
    }
}
