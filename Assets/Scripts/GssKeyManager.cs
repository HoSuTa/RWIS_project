using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GssKeyManager : MonoBehaviour
{
    [SerializeField]
    InputField _gssKeyField;
    const string _gssKeyUrl = "Assets/Resources/gssKey.json";
    public bool _savingIsStillInProcess = false;
    //OnClickで
    public void SaveGssKey()
    {
        if (string.IsNullOrEmpty(_gssKeyField.text))
        {
            Debug.LogError($"<color=blue>[GssKeyManager]</color> field is empty.");
            return;
        }
        //GASとの連携で確認とってからセーブしたりエラーを表示したほうが良いよね.
        _savingIsStillInProcess = true;
        KeyManager.SaveKey(_gssKeyUrl, _gssKeyField.text, EndedKeyManagerFunc);
    }

    private void EndedKeyManagerFunc()
    {
        _savingIsStillInProcess = false;
        Debug.Log($"<color=blue>[GssKeyManager]</color> Process has ended.");
    }
}
