using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GssDbManageWrapper;

public static class GssKeyManager
{
    public static void SaveGssKey(string gssUrl)
    {
        if (string.IsNullOrEmpty(gssUrl))
        {
            Debug.LogError($"<color=blue>[GssKeyManager]</color> field is empty.");
            return;
        }

        KeyManager.SaveKey(KeyManager.GSS_URL_PATH, gssUrl);

        //Debug.LogError($"<color=blue>[GssKeyManager]</color> gssUrl=\"{gssUrl}\" cannnot be recognised by GSS.");
    }

    public static string GetGssKey()
    {
        return KeyManager.GetKeyData(KeyManager.GSS_URL_PATH);
    }

    public static bool IsGssUrlAssigned()
    {
        var gssUrl = GetGssKey();
        return !string.IsNullOrEmpty(gssUrl);
    }

    private static bool IsGssUrlUsable()
    {
        return true;
    }
}
