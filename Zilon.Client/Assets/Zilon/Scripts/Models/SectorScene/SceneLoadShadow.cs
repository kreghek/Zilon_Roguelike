using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;
using UnityEngine.UI;

public class SceneLoadShadow : MonoBehaviour
{
    private const float SHADOW_DURATION_SECONDS = 3f;

    private float _counter;

    public AnimationCommonBlocker ShadowBlocker { get; private set; }

    public Image ShadowImage;

    public void Init(AnimationCommonBlocker shadowBlocker)
    {
        if (shadowBlocker is null)
        {
            throw new System.ArgumentNullException(nameof(shadowBlocker));
        }

        ShadowBlocker = shadowBlocker;
    }

    void Update()
    {
        if (ShadowBlocker is null)
        {
            return;
        }

        _counter += Time.deltaTime;

        var progress = _counter / SHADOW_DURATION_SECONDS;
        ShadowImage.color = new Color(0, 0, 0, 1 - Mathf.Sin(Mathf.PI / 2 * progress));

        if (_counter >= SHADOW_DURATION_SECONDS)
        {
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        if (ShadowBlocker != null)
        {
            ShadowBlocker.Release();
        }
    }
}
