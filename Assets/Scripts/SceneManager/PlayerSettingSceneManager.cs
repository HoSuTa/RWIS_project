using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GssDbManageWrapper;

[RequireComponent(typeof(GssDbHub))]
[RequireComponent(typeof(UserDataManager))]
[RequireComponent(typeof(PlayerSettingUIManager))]
[RequireComponent(typeof(LonLatGetter))]
public class PlayerSettingSceneManager : MonoBehaviour
{
    PlayerSettingUIManager _playerSettingUIManager;
    LonLatGetter _lonLatGetter;
    GssDbHub _gssDbHub;
    UserDataManager _userDataManager;

    private void Awake()
    {
        if (_playerSettingUIManager == null) _playerSettingUIManager = GetComponent<PlayerSettingUIManager>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();
        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
    }

    public void LoadGameScene()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("TestingAlgo");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var userDataManager = GameObject.FindWithTag("Manager").GetComponent<UserDataManager>();
        var mapboxManager = GameObject.FindWithTag("Manager").GetComponent<MapboxMapManager>();


        userDataManager._localPlayer = _userDataManager._localPlayer;
        mapboxManager._lonLatFromPlayerSetting = new Vector2(_lonLatGetter.Latitude, _lonLatGetter.Longitude);

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
