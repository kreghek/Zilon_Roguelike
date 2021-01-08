using System;

using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

using Zilon.Core.Components;

public class ActorGraphicBase : MonoBehaviour
{
    private GameObject _rootObject;
    private bool _isRootRotting;
    private ICommandBlocker _hitAnimationBlocker;

    [Inject] private readonly IAnimationBlockerService _animationBlockerService;

    public Animator Animator;

    public virtual VisualPropHolder GetVisualProp(EquipmentSlotTypes types)
    {
        throw new NotImplementedException();
    }

    public virtual void ProcessHit(Vector3 targetPosition)
    {
        if (_hitAnimationBlocker is null)
        {
            _hitAnimationBlocker = new TimeLimitedAnimationBlocker();
            _animationBlockerService.AddBlocker(_hitAnimationBlocker);
        }

        PlayHit();
        RotateTo(targetPosition);
    }

    public virtual void ProcessDeath(GameObject rootObject, bool isRootRotting)
    {
        _rootObject = rootObject;
        _isRootRotting = isRootRotting;
        PlayDeath();
        FinishHitAnimation();
    }

    public virtual void ProcessMove(Vector3 targetPosition)
    {
        RotateTo(targetPosition);
    }

    public virtual void ProcessMine(Vector3 targetPosition)
    {
        var diffPosition = transform.position - targetPosition;
        Animator.SetFloat("targetX", diffPosition.x);
        Animator.SetFloat("targetY", diffPosition.y);

        RotateTo(targetPosition);

        Animator.Play("Mine");
    }

    public virtual void ProcessInteractive(Vector3 targetPosition)
    {
        RotateTo(targetPosition);
    }

    private void PlayHit()
    {
        Animator.Play("HumanoidAttack", -1, 0);
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
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
            Justification = "Called as animation event handler")]
    private void StartRotting()
    {
        var corpse = gameObject.AddComponent<Rotting>();
        corpse.RootObject = _rootObject;
        corpse.RootRotting = _isRootRotting;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
        Justification = "Called as animation event handler")]
    private void FinishHitAnimation()
    {
        if (_hitAnimationBlocker != null)
        {
            _hitAnimationBlocker.Release();
            _hitAnimationBlocker = null;
        }
    }

    public void OnDestroy()
    {
        FinishHitAnimation();
    }
}