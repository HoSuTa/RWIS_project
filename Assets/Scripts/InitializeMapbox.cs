using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class InitializeMapbox : MonoBehaviour
{
    [SerializeField] AbstractMap map;

    // Start is called before the first frame update
    void Start()
    {
        
        Vector2d lonlat_pos = new Vector2d();
        lonlat_pos.x = 35.5539161;
        lonlat_pos.y = 139.6470643;
        map.Initialize(lonlat_pos, 16);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
