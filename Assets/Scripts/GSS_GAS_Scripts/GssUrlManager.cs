using UnityEngine;

public static class GssUrlManager
{
    public static void SaveUrl(string gssUrl)
    {
        if (string.IsNullOrEmpty(gssUrl))
        {
            Debug.LogError($"<color=blue>[GssUrlManager]</color> field is empty.");
            return;
        }

        KeyManager.SaveKey(KeyManager.GSS_URL_PATH, gssUrl);
    }

    public static string GetUrl()
    {
        return KeyManager.GetKeyData(KeyManager.GSS_URL_PATH);
    }

    public static bool IsUrlAssigned()
    {
        var gssUrl = GetUrl();
        return !string.IsNullOrEmpty(gssUrl);
    }

    public static void ResetUrl()
    {
        KeyManager.RemoveKeyFile(KeyManager.GSS_URL_PATH);
    }
}
