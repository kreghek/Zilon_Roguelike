using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

public class GameLoopUpdater : MonoBehaviour
{
    [NotNull] [Inject] private readonly GlobeStorage _globeStorage;
    [NotNull] [Inject] private readonly ICommandBlockerService _commandBlockerService;

    void Start()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        StartGameLoopUpdate();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private async Task StartGameLoopUpdate()
    {
        while (true)
        {
            await _globeStorage.Globe.UpdateAsync();
            await _commandBlockerService.WaitBlockers();
        }
    }
}
