using UnityEngine;

public static class GasUrlManager
{
    public static void SaveGasUrl(string gasUrl)
    {
        if (string.IsNullOrEmpty(gasUrl))
        {
            Debug.LogError($"<color=blue>[GasUrlManager]</color> field is empty.");
            return;
        }

        KeyManager.SaveKey(KeyManager.GAS_URL_PATH, gasUrl);
    }

    public static string GetGasUrl()
    {
        return KeyManager.GetKeyData(KeyManager.GAS_URL_PATH);
    }

    public static bool IsGasUrlAssigned()
    {
        var gasUrl = GetGasUrl();
        return !string.IsNullOrEmpty(gasUrl);
    }

    public static void ResetGasUrl()
    {
        KeyManager.RemoveKeyFile(KeyManager.GSS_URL_PATH);
    }
}

