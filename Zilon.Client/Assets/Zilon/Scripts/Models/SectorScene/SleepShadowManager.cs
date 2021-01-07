using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

public class SleepShadowManager : MonoBehaviour
{
    [Inject] private readonly IAnimationBlockerService _commandBlockerService;

    public SleepShadow SleepShadow;

    public void StartShadowAnimation()
    {
        var sleepBlocker = new AnimationCommonBlocker();
        _commandBlockerService.AddBlocker(sleepBlocker);

        var sleepShadow = Instantiate(SleepShadow, transform);
        sleepShadow.Init(sleepBlocker);
    }
}
