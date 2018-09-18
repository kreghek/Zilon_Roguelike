using System;

using UnityEngine;

public class HumanoidActorGraphic : ActorGraphicBase
{
    private bool _isDead;

    private float _rotationCounter;

    public VisualPropHolder[] VisualHolders;
    
    public override void ProcessDeath()
    {
        _isDead = true;
    }

    public void Update()
    {
        if (!_isDead)
        {
            _rotationCounter += Time.deltaTime * 3;
            var angle = (float) Math.Sin(_rotationCounter);

            transform.Rotate(Vector3.back, angle * 0.3f);
        }
    }
}
