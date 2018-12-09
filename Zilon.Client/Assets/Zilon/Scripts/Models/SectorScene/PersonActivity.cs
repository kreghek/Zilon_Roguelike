using UnityEngine;

public class PersonActivity : MonoBehaviour
{
    public Animator Animator;

    public void PlayHit()
    {
        Animator.Play("HumanoidAttack");
    }
}
