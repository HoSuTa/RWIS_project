using UnityEngine;

public static class GssUrlManager
{
    public static void SaveGssUrl(string gssUrl)
    {
        if (string.IsNullOrEmpty(gssUrl))
        {
            Debug.LogError($"<color=blue>[GssUrlManager]</color> field is empty.");
            return;
        }

        KeyManager.SaveKey(KeyManager.GSS_URL_PATH, gssUrl);
    }

    public static string GetGssUrl()
    {
        return KeyManager.GetKeyData(KeyManager.GSS_URL_PATH);
    }

    public static bool IsGssUrlAssigned()
    {
        var gssUrl = GetGssUrl();
        return !string.IsNullOrEmpty(gssUrl);
    }

    public static void ResetGssUrl()
    {
        KeyManager.RemoveKeyFile(KeyManager.GSS_URL_PATH);
    }
}
