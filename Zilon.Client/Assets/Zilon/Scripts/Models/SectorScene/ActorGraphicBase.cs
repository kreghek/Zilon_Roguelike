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

    public virtual void ProcessHit()
    {
        PlayHit();
    }

    public virtual void ProcessDeath(GameObject rootObject, bool isRootRotting)
    {
        _rootObject = rootObject;
        _isRootRotting = isRootRotting;
        PlayDeath();
    }

    private void PlayHit()
    {
        Animator.Play("HumanoidAttack");
    }

    private void PlayDeath()
    {
        Animator.Play("HumanoidDeath");
    }

    private void StartRotting()
    {
        var corpse = gameObject.AddComponent<Rotting>();
        corpse.RootObject = _rootObject;
        corpse.RootRotting = _isRootRotting;
    }
}