using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

public class GssLocalManager : MonoBehaviour
{
    GssDbHub _gssDbHub;
    LocalDataHub _localDataHub;

    private void Start()
    {
        _gssDbHub = new GssDbHub();
        _localDataHub = new LocalDataHub();
    }
}
