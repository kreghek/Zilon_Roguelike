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

        var f = _counter * SPEED_COEF;
        var diff = Mathf.Sin(f) * AMPLITUDE;
        var tremorPosition = Vector3.up * diff;
        Target.transform.localPosition = _initPosition + tremorPosition;
    }
}
