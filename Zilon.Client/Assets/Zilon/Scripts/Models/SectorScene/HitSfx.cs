using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;

public class HitSfx : MonoBehaviour
{
    private const float _fadeSpeed = 2;

    private float _lifetimeCounter;

    [NotNull] public SpriteRenderer EffectSpriteRenderer;

    public Sprite MeleeSprite;
    public Sprite ShootSprite;

    public IList<HitSfx> HitSfxes { get; set; }

    public HitSfx()
    {
        _lifetimeCounter = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        _lifetimeCounter -= Time.deltaTime * _fadeSpeed;
        EffectSpriteRenderer.color = new Color(1, 1, 1, _lifetimeCounter);

        if (_lifetimeCounter <= 0)
        {
            HitSfxes.Remove(this);
            Destroy(gameObject);
        }
    }
}
