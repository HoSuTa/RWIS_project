namespace Mapbox.Unity.Map
{
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Mapbox.Unity.Location;
    using UnityEngine;
    using UnityEngine.UI;
    using Mapbox.Utils;
    using System.Drawing;

    public class InitializeMapWithRwis : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        private Text text;

        ILocationProvider _locationProvider;

        [SerializeField] private LonLatGetter lonLatGetter;

        [SerializeField]
        private double[] initialLonLats;
        [SerializeField]
        private int initialZoom;
        [SerializeField]
        private double minLength;
        [SerializeField]
        private List<Vector2> lonLatsVertices;
        [SerializeField]
        private List<Vector3> list;

        [SerializeField] private float lineWidth = 2.0f;

        private LineRenderer lineRend;
        private int positionCount;

        private Slider ZoomSlider;


        private void Awake()
        {
            // Prevent double initialization of the map. 
            _map.InitializeOnStart = false;


        }

        void Update()
        {

            var closedLine = GetComponent<ClosedLine>();
            closedLine.vertices = list;
            var indices = closedLine.GetClosedIndices();

            if (indices != null && indices.Count >= 3)
            {
                GetComponent<LineAreaCalculator>().indices = indices;
               // text.text = $"Score: {GetComponent<LineAreaCalculator>().CalulateArea()}";
            }

            //_map.AbsoluteZoom  = ZoomSlider.value;
            Debug.Log(_map.Zoom);
        }


        void Start()
        {

            var lineRenderer = GetComponent<WorldLineRenderer>();
            lineRenderer.lonLatsVertices = lonLatsVertices;
            var areaCalculator = GetComponent<LineAreaCalculator>();
            areaCalculator.vertices = lonLatsVertices;
            var closedLine = GetComponent<ClosedLine>();
           // closedLine.vertices = lonLatsVertices;
            closedLine.minLength = minLength;

            lineRend = gameObject.AddComponent<LineRenderer>();
            lineRend.useWorldSpace = false;
            lineRend.SetWidth(lineWidth, lineWidth);

            positionCount = 0;

            StartCoroutine(Map_Location());
            StartCoroutine(Save_Location());

            ZoomSlider = GameObject.Find("Slider").GetComponent<Slider>();

            float maxZoom = 19f;
            float nowZoom = 18f;


            //スライダーの最大値の設定
            ZoomSlider.maxValue = maxZoom;

            //スライダーの現在値の設定
            ZoomSlider.value = nowZoom;
        }

        protected IEnumerator Save_Location()
        {
            while (true)
            {
                if (lonLatGetter.CanGetLonLat())
                {
                    Vector2d LL = new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude);

                    Vector3 LL_3 = _map.GeoToWorldPosition(LL);

                    LL_3 = LL_3 + new Vector3(0, 1, 0);

                    list.Add(LL_3);
                    //Debug.Log(LL_3);

                    positionCount++;
                    lineRend.positionCount = positionCount;
                    lineRend.SetPosition(positionCount - 1, list[positionCount - 1]);
          

                    //Instantiate( GameObject.CreatePrimitive(PrimitiveType.Capsule), LL_3, Quaternion.identity);


                }
                yield return new WaitForSeconds(5.0f);
            }


        }

        protected IEnumerator Map_Location()
        {
            while (true)
            {
                if (lonLatGetter.CanGetLonLat())
                {
                    Vector2d LL = new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude);

                    _map.Initialize(LL, _map.AbsoluteZoom);

           
                    break;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
       

    }


}