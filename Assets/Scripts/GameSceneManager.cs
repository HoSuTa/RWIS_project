using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GssDbManageWrapper;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    PlayerSettingUIManager _playerSettingUIManager;

    private void Awake()
    {
        if (_playerSettingUIManager == null) _playerSettingUIManager = GetComponent<PlayerSettingUIManager>();
    }


    public void LoadGameScene()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("TestingGame");
    }

    public void LoadPlayerSettingScene()
    {
        SceneManager.LoadScene("PlayerSetting");
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var userDataManager = GameObject.FindWithTag("Manager").GetComponent<UserDataManager>();

        // データを渡す処理
        userDataManager.LocalPlayerName = _playerSettingUIManager._playerNameField.text;

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
