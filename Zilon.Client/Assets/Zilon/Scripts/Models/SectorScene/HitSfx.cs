using JetBrains.Annotations;

using UnityEngine;

public class HitSfx : MonoBehaviour
{
    private const float _fadeSpeed = 2;

    private float _lifetimeCounter;

    [NotNull] public SpriteRenderer EffectSpriteRenderer;

    public HitSfx()
    {
        _lifetimeCounter = 1;
    }

    // Update is called once per frame
    void Update()
    {
        _lifetimeCounter -= Time.deltaTime * _fadeSpeed;
        EffectSpriteRenderer.color = new Color(1, 1, 1, _lifetimeCounter);

        if (_lifetimeCounter <= 0)
        {
            Destroy(gameObject);
        }
    }
}
