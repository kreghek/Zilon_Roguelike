using System;

using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    private Camera _targetCamera;
    private float _startSize;
    private float _mixSize;
    private float _maxSize;

    public void Awake()
    {
        _targetCamera = Camera.main;
        _startSize = 4;
        _mixSize = 3;
        _maxSize = 8;

        _targetCamera.orthographicSize = _startSize;
    }

    void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Math.Abs(scroll) < 0.0001)
        {
            return;
        }

        if (scroll > 0)
        {
            if (_targetCamera.orthographicSize < _maxSize)
            {
                _targetCamera.orthographicSize += 1.0f;
            }
        }
        else
        {
            if (_targetCamera.orthographicSize > _mixSize)
            {
                _targetCamera.orthographicSize -= 1.0f;
            }
        }
    }
}
