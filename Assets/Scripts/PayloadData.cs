﻿using System;
using UnityEngine;

namespace GssDbManageWrapper
{
    [Serializable]
    public class PayloadData
    {
        public string userName;
        public string message;

        public override string ToString()
        {
            return $"userName : {this.userName}, message : {this.message}";
        }
    }

    [Serializable]
    public class MessageJson
    {
        public int areaId;
        public int vertexId;
        public Vector2 lonLat;

        public override string ToString()
        {
            return $"areaId={this.areaId}, vertexId={this.vertexId}, lonLat={this.lonLat}.";
        }
    }
}