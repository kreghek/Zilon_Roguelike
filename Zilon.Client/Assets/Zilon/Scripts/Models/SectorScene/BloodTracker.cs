using UnityEngine;

public sealed class BloodTracker : MonoBehaviour
{
    private const float MAX_OFFSET = 0.5f;
    public Sprite BloodSprite;

    public void Init(Transform sectorViewModelTransform, ActorViewModel damagedActorViewModel)
    {
        transform.SetParent(sectorViewModelTransform);

        var randomOffset = Random.insideUnitCircle * MAX_OFFSET;
        var offsetVector = new Vector3(randomOffset.x, randomOffset.y);
        transform.position = damagedActorViewModel.transform.position + offsetVector;
        transform.Rotate(Vector3.forward, Random.Range(0, 360));
    }
}

