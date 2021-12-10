using System;
using UnityEngine;

public static class KeyManager
{
    //https://blog.mbaas.nifcloud.com/entry/9044
    public static string GetGasUrl(string jsonFileDir)
    {
        if (string.IsNullOrEmpty(jsonFileDir))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> {nameof(jsonFileDir)} is empty.");
            return null;
        }

        TextAsset textAsset = Resources.Load(jsonFileDir) as TextAsset;
        if (textAsset == null)
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> could not load the TextAsset.");
            return null;
        }

        var keys = JsonUtility.FromJson<Keys>(textAsset.text);
        return keys.gasUrl;

    }

    [Serializable]
    private class Keys
    {
        public string gasUrl;
    }
}
