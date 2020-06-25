using UnityEngine;

public class AniHumanoidActorGraphic : ActorGraphicBase
{
    public override void ProcessMove(Vector3 targetPosition)
    {
        RotateTo(targetPosition);
        Animator.Play("HumWalk");
    }

    public override void ProcessHit(Vector3 targetPosition)
    {
        RotateTo(targetPosition);
        Animator.Play("HumMainHandAttack");
    }
}
