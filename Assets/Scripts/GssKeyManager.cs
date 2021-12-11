using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GssKeyManager : MonoBehaviour
{
    [SerializeField]
    InputField _gssKeyField;

    private bool _savingIsStillInProcess = false;

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
        if(IsGssKeyUsable())
        {
            KeyManager.SaveKey(KeyManager.GSS_KEY_PATH, _gssKeyField.text, EndedKeyManagerFunc);
        }
        else
        {
            Debug.LogError($"<color=blue>[GssKeyManager]</color> gssKey=\"{_gssKeyField.text}\" cannnot be recognised by GSS.");
        }
    }

    public string GetGssKey()
    {
        return KeyManager.GetKeyData(KeyManager.GSS_KEY_PATH, EndedKeyManagerFunc);
    }

    private void EndedKeyManagerFunc()
    {
        _savingIsStillInProcess = false;
        Debug.Log($"<color=blue>[GssKeyManager]</color> Process has ended.");
    }

    public bool IsStillInProcess()
    {
        return _savingIsStillInProcess;
    }

    public bool IsGssKeyAssigned()
    {
        var gssKey = GetGssKey();
        return !string.IsNullOrEmpty(gssKey);
    }

    private bool IsGssKeyUsable()
    {
        return true;
    }
}
