using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
using UnityEngine.UI;  
 
public class ScoreManager : MonoBehaviour
{


    public GameObject score_object = null; // Textオブジェクト
    public float score_num = 0; // スコア変数

    // 初期化
    void Start()
    {
    }

    // 更新
    void Update()
    {
        // オブジェクトからTextコンポーネントを取得
        Text score_text = score_object.GetComponent<Text>();
        // テキストの表示を入れ替える

        score_num = Addscore(score_num);

        score_text.text = "My Score : " + score_num;
    }

    float Addscore(float add_score)
    {
        float score_sum = 0;

        score_sum += add_score;

        return score_sum;
    }

}