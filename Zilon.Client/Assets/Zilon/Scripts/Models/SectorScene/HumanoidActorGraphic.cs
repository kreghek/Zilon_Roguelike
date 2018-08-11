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

    public override VisualPropHolder GetVisualProp(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0: // оружие
                return VisualHolders[0];

            case 1: // щит
                return VisualHolders[1];

            case 2: // тело
                return VisualHolders[2];

            case 3: // голова
                return VisualHolders[3];
            
            default:
                throw new ArgumentException(nameof(slotIndex));
        }
    }
}
