using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour
{
    private const float cameraSpeed = 0.5f;

    private Vector3 _targetPosition;

    public void Start()
    {
        _targetPosition = transform.position;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _targetPosition += Vector3.left * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _targetPosition -= Vector3.left * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            _targetPosition += Vector3.up * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _targetPosition -= Vector3.up * cameraSpeed;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _targetPosition, Time.deltaTime * 5);
    }

}