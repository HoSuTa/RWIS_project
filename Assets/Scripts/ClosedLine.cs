﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedLine : MonoBehaviour
{
    // Start is called before the first frame update
    // Observed
    // List<Vector2> vertices <- 時系列で記録される点列
    // List<List<Vector2>> closePointsLists;
    // 問題は...点列->閉凸多角形にすること
    //
    // Algorithm 0: 開始点との最近傍探索
    //
    // 開始点
    // Lineとしての自己
    // t0......t1
    //   \       \
    //    \       \ 
    //     \       t2
    //      \      /
    //       \    /
    //        \t3
    // Algorithm 1: 任意点との最近傍探索
    //
    // Indices 1->2->3
    // t0.......t1
    //         |  \
    //         |   \ 
    //         |    t2
    //         |    /
    //         |   /
    //          t3
    //
    // Algorithm 2: Lineの自己交差
    //
    // Indices 1->2->3
    //   t4
    //    \
    // t0..t4'...t1
    //      \     \
    //       \     \ 
    //        \    t2
    //         \    /
    //          \  /
    //          t3
    //
    // Algorithm 3: 他の閉多角形との交差
    //                     
    //                   t0
    //    s0......s1    /
    //      \        \ /
    //       \        s2
    //        \      /
    //         \    /
    //          \  /
    //          s3
    //           
    // Issue どういったメソッドにしておくとつかいやすいのか?
    //
    // bool IsClosed(out List<UINT> indices){ }
    // true -> trueにしたうえでindicesの閉曲面点のindexを返す
    //
    // bool AddPoint(const Vector2 newPoint, out List<UINT> indices){}
    [SerializeField]
    private List<Vector2> vertices;
    // 
    [SerializeField]
    private float minLength = 0.01f;
    List<uint> GetClosedIndices(){
        bool isClosed =  Vector2.Distance(vertices[vertices.Count-1],vertices[0])<minLength;
        if (isClosed){
            List<uint> indices = new List<uint>();
            for (var i = 0;i<vertices.Count;++i){
                indices[i] = (uint)i;
            }
            return indices;
        }
        return null;
    }
    void Start()
    {
    }
    void Update()
    {
    }
}
