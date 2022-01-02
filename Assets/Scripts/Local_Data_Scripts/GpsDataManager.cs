using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;
using UnityEngine.UI;
using System.Linq;


[RequireComponent(typeof(GssDbHub))]
[RequireComponent(typeof(LonLatGetter))]
[RequireComponent(typeof(LocalDataHub))]
[RequireComponent(typeof(UserDataManager))]

public class GpsDataManager : MonoBehaviour
{
    GssDbHub _gssDbHub;
    LonLatGetter _lonGetter;
    LocalDataHub _localDataHub;
    UserDataManager _userDataManager;

    private void Awake()
    {
        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_lonGetter == null) _lonGetter = GetComponent<LonLatGetter>();
        if (_localDataHub == null) _localDataHub = GetComponent<LocalDataHub>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
    }

    

    





}
