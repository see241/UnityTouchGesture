using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Callback();

public delegate void CallbackI1(int index);

public delegate void CallbackF1(float f);

public class Gesture : MonoBehaviour
{
    public static Gesture Instance;
    private Vector2 _dirVector;
    private IEnumerator _iGestureRecognize;

    private readonly int[] _anchoredDegree = new int[8] { 0, 45, 90, 135, 180, 225, 270, 315 };
    private List<int> _deltaDegrees = new List<int>();

    private bool[,] _usedDegrees = new bool[2, 8]
    {
        {false,false,false,false,false,false,false,false},
        {false,false,false,false,false,false,false,false}
    };

    public CallbackI1 onTouchStart;
    public CallbackI1 onTouching;
    public CallbackI1 onTouchEnd;
    public CallbackI1 onNoteActive;

    private int[] _inputedDegrees = new int[2];
    private readonly float _screenHalfHeight = Screen.height / 2f;
    private readonly float _screenHalfWidth = Screen.width / 2f;
    private float[] _touchTime = new float[2];
    private bool[] _isRotateDetected = new bool[2];
    private float[] _lastTouchTime = new float[2];
    private Vector2[] _touchStartPosition = new Vector2[2];

    private Vector2[] _touchEndPosition = new Vector2[2];

    [HideInInspector]
    public bool[] isTouching = new bool[2] { false, false };

    private const float swipeSensitive = 4f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        isTouching = new bool[2] { false, false };
    }

    private void Start()
    {
        GameManager.Instance.isPause = false;
        onTouchStart += InitializeGesture;
        onTouching += ProcessTouching;
        onTouchEnd += ProcessTouchEnd;
        onTouchStart += (index) =>
        {
        };
        onTouchEnd += (index) =>
        {
        };
    }

    private void Update()
    {
        if (Input.touchCount > 0 && GameManager.Instance.isPause == false)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (i > 2) break;
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                    StartCoroutine(GestureRecognize(i));
            }
        }
    }

    // 노트 처리

    // 터치 처리 함수 (터치중)
    private void ProcessTouching(int fingerId)
    {
        if (CalculateCircleGesture(fingerId))
        {
            // Rotate 제스쳐 처리
            // DoRotate();

            // 연속 Rotate 처리를 위한 제스쳐 정보 초기화
            InitializeGesture(fingerId);
            _isRotateDetected[fingerId] = true;
            return;
        }
    }

    // 터치 처리 함수 (터치종료)
    private void ProcessTouchEnd(int fingerId)
    {
        // 로테이트 제스쳐가 감지됐을 경우 함수 종료
        if (_isRotateDetected[fingerId])
        {
            _isRotateDetected[fingerId] = false;
            return;
        }
        // 터치 시간이 길 경우 롱터치로 인식, 함수 종료
        if (_touchTime[fingerId] > 0.4f)
        {
            return;
        }
        // 스와이프 처리 시작
        if (_touchEndPosition[fingerId].x - _touchStartPosition[fingerId].x > _screenHalfWidth / swipeSensitive)
        {
            // Swipe 제스쳐 처리
            // DoSwipe();
            return;
        }

        if ((_touchEndPosition[fingerId].x - _touchStartPosition[fingerId].x) * -1 > _screenHalfWidth / swipeSensitive)
        {
            // Swipe 제스쳐 처리
            // DoSwipe();
            return;
        }

        if (_touchEndPosition[fingerId].y - _touchStartPosition[fingerId].y > _screenHalfHeight / swipeSensitive)
        {
            // Swipe 제스쳐 처리
            // DoSwipe();
            return;
        }
        if ((_touchEndPosition[fingerId].y - _touchStartPosition[fingerId].y) * -1 > _screenHalfHeight / swipeSensitive)
        {
            // Swipe 제스쳐 처리
            // DoSwipe();
            return;
        }
        // 스와이프 처리 끝

        // 위의 조건을 모두 만족하지 않을경우 텝 노트 처리

        // Tap 제스쳐 처리
        // DoTap();
    }

    // 터치 정보 초기화 함수
    private void InitializeGesture(int fingerId)
    {
        for (int i = 0; i < 8; i++)
        {
            _usedDegrees[fingerId, i] = false;
        }

        _touchTime[fingerId] = 0;
        _inputedDegrees[fingerId] = 0;
    }

    // 각도 8각 변환 함수
    private int CalculateAnchoredDegree(float angle, int fingerId)
    {
        float min = int.MaxValue;
        var minIndex = 0;
        for (int i = 0; i < _anchoredDegree.Length; i++)
        {
            var difAngle = angle - _anchoredDegree[i];
            if (difAngle < 0) difAngle *= -1;// reverse negative number
            if (difAngle < min)
            {
                min = difAngle;
                minIndex = i;
            }
        }
        if (_usedDegrees[fingerId, minIndex] == false)
        {
            _inputedDegrees[fingerId] += _anchoredDegree[minIndex];
            _usedDegrees[fingerId, minIndex] = true;
        }

        return _anchoredDegree[minIndex];
    }

    // 로테이트 제스쳐 처리 함수
    // 변환된 8각을 기준으로 터치 각도의 합을 통해 로테이트 여부를 확인
    private bool CalculateCircleGesture(int fingerId)
    {
        return _inputedDegrees[fingerId] >= 945;
    }

    private IEnumerator GestureRecognize(int touchIndex)
    {
        Touch touch = Input.GetTouch(touchIndex);
        // 손가락 식별을 위해 fingerId를 저장
        int fingerId = touch.fingerId;
        Vector2 deltaTouch = touch.position;
        // 터치 시작 delegate 호출
        onTouchStart(fingerId);
        _touchStartPosition[fingerId] = touch.position;

        while (touch.phase != TouchPhase.Ended)
        {
            // touchIndex가 바뀔 경우 fingerId를 기반으로
            // 현재 터치에서 fingerId를 바탕으로 바뀐 touchIndex를 찾음
            if (touchIndex >= Input.touchCount)
            {
                bool isFind = false;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (fingerId == Input.GetTouch(i).fingerId)
                    {
                        touchIndex = i;
                        isFind = true;
                    }
                }

                if (!isFind)
                    break;
            }
            touch = Input.GetTouch(touchIndex);
            // TouchIndex의 fingerId와 저장된 fingerId가 다를 경우
            // 현재 터치에서 fingerId를 바탕으로 바뀐 touchIndex를 찾음
            if (touch.fingerId != fingerId)
            {
                bool isFind = false;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (fingerId == Input.GetTouch(i).fingerId)
                    {
                        touchIndex = i;
                        isFind = true;
                        touch = Input.GetTouch(touchIndex);
                    }
                }

                if (!isFind)
                    break;
            }

            _touchTime[fingerId] += Time.deltaTime;
            Vector2 touchPos = touch.position;

            // deltaTouch간의 각도를 구함
            var dragAngle = Mathf.Atan2(deltaTouch.y - touchPos.y, deltaTouch.x - touchPos.x) * 180 / Mathf.PI + 90;
            // 구해진 각도가 -180~180이기 때문에 계산의 편의를 위해 0~360으로 변환시킴
            if (dragAngle < 0)
                dragAngle += 360f;
            deltaTouch = _touchEndPosition[fingerId] = touch.position;
            CalculateAnchoredDegree(dragAngle, fingerId);
            onTouching(fingerId);
            yield return null;
        }
        onTouchEnd(fingerId);
    }
}