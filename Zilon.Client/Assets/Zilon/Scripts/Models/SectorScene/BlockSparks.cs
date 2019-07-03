using UnityEngine;

public class BlockSparks : MonoBehaviour
{
    private const float LIFETIME = 1;

    private float _lifetmeCounter;

    public void FixedUpdate()
    {
        _lifetmeCounter += Time.deltaTime;
        if (_lifetmeCounter >= LIFETIME)
        {
            Destroy(gameObject);
        }
    }
}
