namespace Mapbox.Unity.Map
{
    using System.Collections;
    using Mapbox.Unity.Location;
    using UnityEngine;
    using Mapbox.Utils;

    public class InitializeMapWithRwis : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        ILocationProvider _locationProvider;
        [SerializeField]  private LonLatGetter lonLatGetter; 


        private void Awake()
        {
            // Prevent double initialization of the map. 
            _map.InitializeOnStart = false;
           

        }

        protected IEnumerator Start()
        {
            while (true)
            {
                if (lonLatGetter.CanGetLonLat())
                {
                    _map.Initialize(new Vector2d(lonLatGetter.Latitude, lonLatGetter.Longitude), _map.AbsoluteZoom);
                    break;
                }
                yield return new WaitForSeconds(1.0f);

            }
        }
    }
}