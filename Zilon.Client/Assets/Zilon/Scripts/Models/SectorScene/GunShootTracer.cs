using JetBrains.Annotations;

using UnityEngine;

public class GunShootTracer : MonoBehaviour
{
    private float _counter = 0.05f;

    public LineRenderer LineRenderer;

    public Vector3 TargetPosition { get; set; }
    public Vector3 FromPosition { get; set; }

    [UsedImplicitly]
    public void Start()
    {
        LineRenderer.SetPosition(0, FromPosition + Vector3.back * 5);
        LineRenderer.SetPosition(1, TargetPosition + Vector3.back * 5);
    }

    [UsedImplicitly]
    public void Update()
    {
        _counter -= Time.deltaTime;
        if (_counter <= 0)
        {
            Destroy(gameObject);
        }
    }
}
