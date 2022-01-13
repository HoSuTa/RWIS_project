using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScoreManager : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト
    public float score_num = 0; // スコア変数

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // オブジェクトからTextコンポーネントを取得
        //if (user1 exit){

        Text score_text = score_object.GetComponent<Text>();
        // テキストの表示を入れ替える

        score_num = Addscore(score_num);

        score_text.text = "User1 Score : " + score_num;
    }

    float Addscore(float add_score)
    {
        float score_sum = 0;

        score_sum += add_score;

        return score_sum;
    }
}
