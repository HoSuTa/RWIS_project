using System;
using System.IO;
using UnityEngine;

public static class KeyManager
{
    public const string GSS_KEY_PATH = "Assets/Resources/gssKey.json";
    public const string GAS_URL_PATH = "Assets/Resources/gasUrl.json";
    //https://blog.mbaas.nifcloud.com/entry/9044
    public static string GetKeyData(string filePath, Action feedbackHandler = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> {nameof(filePath)} is empty.");
            return null;
        }

        var streamReader = new StreamReader(filePath);
        string fileData = streamReader.ReadToEnd();
        streamReader.Close();
        if (string.IsNullOrEmpty(fileData))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> could not load the file.");
            return null;
        }

        var keys = JsonUtility.FromJson<Keys>(fileData);
        feedbackHandler?.Invoke();
        return keys.key;
    }

    public static void SaveKey(string filePath, string key, Action feedbackHandler = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> {nameof(key)} is empty.");
            return;
        }

        var jsonData = JsonUtility.ToJson(new Keys(key));
        Debug.Log(jsonData);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(jsonData);
        streamWriter.Flush();
        streamWriter.Close();
        feedbackHandler?.Invoke();
    }

    [Serializable]
    private class Keys
    {
        public string key;

        public Keys(string key)
        {
            this.key = key;
        }
    }
}
