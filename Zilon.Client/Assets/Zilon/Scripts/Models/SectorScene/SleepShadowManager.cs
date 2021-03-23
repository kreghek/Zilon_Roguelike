using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;

using Zenject;

using Zilon.Core.Client.Sector;

public class SleepShadowManager : MonoBehaviour
{
    [Inject] private readonly IAnimationBlockerService _commandBlockerService;

    public SleepShadow SleepShadow;

    public SceneLoadShadow SceneLoadShadow;

    public void StartSleepShadowAnimation()
    {
        var sleepBlocker = new AnimationCommonBlocker();
        _commandBlockerService.AddBlocker(sleepBlocker);

        var sleepShadow = Instantiate(SleepShadow, transform);
        sleepShadow.Init(sleepBlocker);
    }

    public void StartSceneLoadShadowAnimation()
    {
        var sleepBlocker = new AnimationCommonBlocker();
        _commandBlockerService.AddBlocker(sleepBlocker);

        var sleepShadow = Instantiate(SceneLoadShadow, transform);
        sleepShadow.Init(sleepBlocker);
    }
}
