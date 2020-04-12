using System;

using UnityEngine;

using Zilon.Core.Components;

public class ActorGraphicBase : MonoBehaviour
{
    private GameObject _rootObject;
    private bool _isRootRotting;

    public Animator Animator;

    public virtual VisualPropHolder GetVisualProp(EquipmentSlotTypes types)
    {
        throw new NotImplementedException();
    }

    public virtual void ProcessHit(Vector3 targetPosition)
    {
        PlayHit();
        RotateTo(targetPosition);
    }

    public virtual void ProcessDeath(GameObject rootObject, bool isRootRotting)
    {
        _rootObject = rootObject;
        _isRootRotting = isRootRotting;
        PlayDeath();
    }

    public virtual void ProcessMove(Vector3 targetPosition)
    {
        RotateTo(targetPosition);
    }

    public virtual void ProcessInteractive(Vector3 targetPosition)
    {
        RotateTo(targetPosition);
    }

    private void PlayHit()
    {
        Animator.Play("HumanoidAttack");
    }

    private void PlayDeath()
    {
        Animator.Play("HumanoidDeath");
    }

    private void RotateTo(Vector3 targetPosition)
    {
        var direction = transform.position - targetPosition;
        if (direction.x >= 0)
        {
            transform.localScale = new Vector3(3, 3, 1);
        }
        else
        {
            transform.localScale = new Vector3(-3, 3, 1);
        }
    }

#pragma warning disable IDE0051 // Remove unused private members
    private void StartRotting()
#pragma warning restore IDE0051 // Remove unused private members
    {
        // Вызывается, как событие анимации.
        var corpse = gameObject.AddComponent<Rotting>();
        corpse.RootObject = _rootObject;
        corpse.RootRotting = _isRootRotting;
    }
}