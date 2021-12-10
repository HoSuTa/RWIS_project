using System;
using UnityEngine;


[Serializable]
public class PayloadData
{
    public string userName;
    public string message;
}

[Serializable]
public class MessageJson
{
    public int areaId;
    public int vertexId;
    public Vector2 lonLat;
}
