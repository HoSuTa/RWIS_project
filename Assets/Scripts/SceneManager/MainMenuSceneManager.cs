using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GssDbManageWrapper;

public class MainMenuSceneManager : MonoBehaviour
{
    public void LoadPlayerSettingScene()
    {
        SceneManager.LoadScene("PlayerSetting");
    }

}
