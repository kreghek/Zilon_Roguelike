using System;
using UnityEngine;

public class HumanoidActorGraphic : MonoBehaviour
{
    private float _rotationCounter;

    public bool IsDead { get; set; }

    public void Update()
    {
        if (!IsDead)
        {
            _rotationCounter += Time.deltaTime * 3;
            var angle = (float) Math.Sin(_rotationCounter);

            transform.Rotate(Vector3.back, angle * 0.3f);
        }
    }
}
