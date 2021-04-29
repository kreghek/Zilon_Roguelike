using System.Collections.Generic;

using Assets.Zilon.Scripts.Models.SectorScene;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client.Sector;

public class HitSfx : MonoBehaviour
{
    private const float FADE_SPEED = 2;
    private const int SFX_DURATION_SECONDS = 1;

    private float _lifetimeCounter;
    private ICommandBlocker _animationBlocker;

    [NotNull] public SpriteRenderer EffectSpriteRenderer;

    [NotNull] [Inject] private readonly IAnimationBlockerService _animationBlockerService;

    public Sprite MeleeSprite;
    public Sprite ShootSprite;

    public IList<HitSfx> HitSfxes { get; set; }

    public void Start()
    {
        _lifetimeCounter = SFX_DURATION_SECONDS;

        _animationBlocker = new TimeLimitedAnimationBlocker();
        _animationBlockerService.AddBlocker(_animationBlocker);
    }

    // Update is called once per frame
    public void Update()
    {
        _lifetimeCounter -= Time.deltaTime * FADE_SPEED;
        EffectSpriteRenderer.color = new Color(1, 1, 1, _lifetimeCounter);

        if (_lifetimeCounter <= 0)
        {
            HitSfxes.Remove(this);
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        _animationBlocker.Release();
    }
}
