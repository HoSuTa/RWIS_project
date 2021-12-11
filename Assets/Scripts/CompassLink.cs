using System.Collections;
using UnityEngine;

public class CompassLink : MonoBehaviour
{

    [SerializeField] private GameObject _rotationTarget;    //コンパスと動機して回転させるもの

    [SerializeField, Space(15f)] private float _intervalSeconds = 3.0f; //同期する周期
    [SerializeField] private LocationServiceStatus _status;

    private void Start()
    {
        StartCoroutine(InputGPSInfomation());   //起動と同時に動作開始
    }

    private IEnumerator InputGPSInfomation()
    {
        while (true)
        {
            switch (_status)
            {
                case LocationServiceStatus.Stopped:
                    Input.compass.enabled = true;   //コンパスを有効化する
                    Input.location.Start();

                    break;
                case LocationServiceStatus.Running:
                    var compassData = Input.compass.trueHeading;
                    _rotationTarget.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -compassData)); //向きを反映

                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(_intervalSeconds);
        }
    }
}