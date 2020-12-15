using System.Collections;
using System.Collections.Generic;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

public class GlobeSelectionHandler : MonoBehaviour
{
    [NotNull]
    [Inject]
    private readonly GlobeStorage _globeStorage;

    [NotNull]
    [Inject]
    private readonly IGlobeInitializer _globeInitializer;

    [NotNull]
    [Inject]
    private readonly NationalUnityEventService _nationalUnityEventService;

    [NotNull]
    [Inject]
    private readonly IPlayer _player;

    public void SelectGlobeAndGo()
    {
        var globe = _globeInitializer.CreateGlobeAsync("intro").Result;
        _globeStorage.AssignGlobe(globe);
        _nationalUnityEventService.Globe = globe;

        SceneManager.LoadScene("combat");
    }
}
