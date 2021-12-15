﻿namespace Mapbox.Unity.Map
{
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Mapbox.Unity.Location;
    using UnityEngine;
    using UnityEngine.UI;
    using Mapbox.Utils;

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

        Vector2d LL_2 = new Vector2d(0, 0);

        private void Awake()
        {
            // Prevent double initialization of the map. 
            _map.InitializeOnStart = false;


        }

        /*
        void Start()
        {
            var abstractMap = GetComponent<AbstractMap>();
            abstractMap.Initialize(new Vector2d(initialLonLats[0], initialLonLats[1]), initialZoom);

            var lineRenderer = GetComponent<WorldLineRenderer>();
            lineRenderer.lonLatsVertices = lonLatsVertices;
            var areaCalculator = GetComponent<LineAreaCalculator>();
            areaCalculator.vertices = lonLatsVertices;
            var closedLine = GetComponent<ClosedLine>();
            closedLine.vertices = lonLatsVertices;
            closedLine.minLength = minLength;
        }

*/

        void Update()
        {
        
          var indices = GetComponent<ClosedLine>().GetClosedIndices();
            if (indices != null && indices.Count >= 3)
            {
                GetComponent<LineAreaCalculator>().indices = indices;
                text.text = $"Score: {GetComponent<LineAreaCalculator>().CalulateArea()}";
            }


        }


        void Start()
        {

            /*
            var abstractMap = GetComponent<AbstractMap>();
            abstractMap.Initialize(new Vector2d(initialLonLats[0], initialLonLats[1]), initialZoom);
            */
            var lineRenderer = GetComponent<WorldLineRenderer>();
            lineRenderer.lonLatsVertices = lonLatsVertices;
            var areaCalculator = GetComponent<LineAreaCalculator>();
            areaCalculator.vertices = lonLatsVertices;
            var closedLine = GetComponent<ClosedLine>();
            closedLine.vertices = lonLatsVertices;
            closedLine.minLength = minLength;

            StartCoroutine(Map_Location());
            StartCoroutine(Save_Location());
            // StopAllCoroutines();
        }

        protected IEnumerator Save_Location()
        {
            while(true)
            {
                if (lonLatGetter.CanGetLonLat())
                {
                    Vector2d LL = new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude);
                   
                   Vector3 LL_3 = _map.GeoToWorldPosition(LL);

                    list.Add(LL_3);
                    //Instantiate( GameObject.CreatePrimitive(PrimitiveType.Capsule), LL_3, Quaternion.identity);
                    Debug.Log(LL_3);

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
                   // LL_2 = LL;
                   // Vector3 LL_3 = new Vector3(0, 0, 0);
                    _map.Initialize(LL, _map.AbsoluteZoom);

                    //_map.Initialize(new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude), _map.AbsoluteZoom);
                    // LL_3 =_map.GeoToWorldPosition(LL);

                    /*
                    StreamWriter writer = new StreamWriter("RWIS.txt");

                    writer.WriteLine(LL);

                    writer.Close();

                    list.Add(LL_3);*/
                    break;
                }
                yield return new WaitForSeconds(1.0f);
                //yield return null;
               // Debug.Log("TEST");
            }
        }

    }
}