using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Tactics;
using Zilon.Core.World;

public class GlobeSelectionHandler : MonoBehaviour
{
    [NotNull]
    [Inject]
    private readonly IGlobeInitializer _globeInitializer;

    [NotNull]
    [Inject]
    private readonly NationalUnityEventService _nationalUnityEventService;

    public void SelectGlobeAndGo()
    {
        var globe = _globeInitializer.CreateGlobeAsync("intro").Result;
        _nationalUnityEventService.Globe = globe;

        SceneManager.LoadScene("combat");
    }
}
