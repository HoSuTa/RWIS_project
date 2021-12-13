using UnityEngine;

public static class GasUrlManager
{
    public static void SaveUrl(string gasUrl)
    {
        if (string.IsNullOrEmpty(gasUrl))
        {
            Debug.LogError($"<color=blue>[GasUrlManager]</color> field is empty.");
            return;
        }

        KeyManager.SaveKey(KeyManager.GAS_URL_PATH, gasUrl);
    }

    public static string GetUrl()
    {
        return KeyManager.GetKeyData(KeyManager.GAS_URL_PATH);
    }

    public static bool IsUrlAssigned()
    {
        var gasUrl = GetUrl();
        return !string.IsNullOrEmpty(gasUrl);
    }

    public static void ResetUrl()
    {
        KeyManager.RemoveKeyFile(KeyManager.GSS_URL_PATH);
    }
}

