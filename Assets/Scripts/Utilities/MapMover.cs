using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMover : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    private Transform _mainCameraTransform;
    [SerializeField]
    [Range(0f, 1f)]
    private float _speed = .1f;
    private bool _isSwiping;
    private Vector2 _startPos, _currentPos, _diffSwipeVec2;
    private float _differenceDisFloat;
    void Start()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        _mainCameraTransform = _mainCamera.transform;
    }

    void Update()
    {
        MovementControll();
    }

    void FixedUpdate()
    {
        MoveMap();
    }

    void MovementControll()
    {
        //移動
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //マウス左クリック時に始点座標を代入
            _startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //押している最中に今の座標を代入
            _currentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _diffSwipeVec2 = _currentPos - _startPos;

            //スワイプ量によってSpeedを変化させる.この時、絶対値にする。
            _differenceDisFloat = _diffSwipeVec2.x * _diffSwipeVec2.y;
            _differenceDisFloat /= 100;
            _differenceDisFloat = Mathf.Abs(_differenceDisFloat);

            //タップしただけで動いてしまうので、距離が短ければ動かないようにする。
            if (_differenceDisFloat > 1)
            {
                _isSwiping = true;


                //最高速度
                if (_differenceDisFloat > 1f)
                {
                    _differenceDisFloat = 1f;
                }

                //最低速度
                if (_differenceDisFloat < 0.1f)
                {
                    _differenceDisFloat = 0.1f;
                }

            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _isSwiping = false;
        }
    }

    void MoveMap()
    {
        if (_isSwiping == true)
        {
            _mainCameraTransform.position -= new Vector3(
                _diffSwipeVec2.x, 0, _diffSwipeVec2.y) * _speed;
        }
    }

}
