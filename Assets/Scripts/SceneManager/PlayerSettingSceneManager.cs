using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GssDbManageWrapper;

[RequireComponent(typeof(PlayerSettingUIManager))]
[RequireComponent(typeof(LonLatGetter))]
public class PlayerSettingSceneManager : MonoBehaviour
{
    PlayerSettingUIManager _playerSettingUIManager;
    LonLatGetter _lonLatGetter;

    private void Awake()
    {
        if (_playerSettingUIManager == null) _playerSettingUIManager = GetComponent<PlayerSettingUIManager>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();
    }

    public void LoadGameScene()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("TestingGame");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var userDataManager = GameObject.FindWithTag("Manager").GetComponent<UserDataManager>();
        var mapboxManager = GameObject.FindWithTag("Manager").GetComponent<MapboxMapManager>();

        // データを渡す処理
        userDataManager.LocalPlayerName = _playerSettingUIManager._playerNameField.text;
        mapboxManager._lonLatFromPlayerSetting = new Vector2(_lonLatGetter.Latitude, _lonLatGetter.Longitude);

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
