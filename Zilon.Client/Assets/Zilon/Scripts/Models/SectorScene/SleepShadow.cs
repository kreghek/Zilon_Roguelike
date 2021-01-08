using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;
using UnityEngine.UI;

public class SleepShadow : MonoBehaviour
{
    private const float SLEEP_DURATION_SECONDS = 4f;

    private float _counter;

    public AnimationCommonBlocker SleepBlocker { get; private set; }

    public Image ShadowImage;

    public void Init(AnimationCommonBlocker sleepBlocker)
    {
        if (sleepBlocker is null)
        {
            throw new System.ArgumentNullException(nameof(sleepBlocker));
        }

        SleepBlocker = sleepBlocker;
    }

    void Update()
    {
        if (SleepBlocker is null)
        {
            return;
        }

        _counter += Time.deltaTime;

        ShadowImage.color = new Color(0, 0, 0, Mathf.Sin(_counter / SLEEP_DURATION_SECONDS * 2 * Mathf.PI));

        if (_counter >= SLEEP_DURATION_SECONDS)
        {
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        if (SleepBlocker != null)
        {
            SleepBlocker.Release();
        }
    }
}
