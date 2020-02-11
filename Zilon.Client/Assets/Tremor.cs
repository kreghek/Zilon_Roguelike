using UnityEngine;

public class Tremor : MonoBehaviour
{
    private const float SPEED_COEF = 70f;
    private const float AMPLITUDE = 0.01f;
    private float _counter;

    private Vector3 _initPosition;

    public Transform Target;


    public void Start()
    {
        _initPosition = Target.localPosition;
    }

    public void Update()
    {
        _counter += Time.deltaTime;

        Target.transform.localPosition = _initPosition + Vector3.up * Mathf.Sin(_counter * SPEED_COEF) * AMPLITUDE;
    }
}
