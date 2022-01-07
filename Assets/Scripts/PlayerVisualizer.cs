namespace Mapbox.Unity.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Text;
    using Mapbox.Unity.Location;
    using UnityEngine.UI;
    using Mapbox.Utils;


    public class PlayerVisualizer : MonoBehaviour
    {

        [SerializeField] private LonLatGetter lonLatGetter;
        [SerializeField]
        AbstractMap _map;
        public GameObject target;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Save_Location());
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected IEnumerator Save_Location()
        {
            
            while (true)
            {
                if (lonLatGetter.CanGetLonLat())
                {
                    Vector2d LL = new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude);

                    Vector3 LL_3 = _map.GeoToWorldPosition(LL);

                    this.transform.position = LL_3;
                }
                yield return new WaitForSeconds(1.0f);
            }


        }



    }
}
