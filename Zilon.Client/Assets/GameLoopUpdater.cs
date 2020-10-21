using System;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

public class GameLoopUpdater : MonoBehaviour
{
    [NotNull] [Inject] private readonly GlobeStorage _globeStorage;
    [NotNull] [Inject] private readonly ICommandBlockerService _commandBlockerService;

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var cancelToken = _cancellationTokenSource.Token;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        StartGameLoopUpdate(cancelToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task StartGameLoopUpdate(CancellationToken cancelToken)
    {
        while (true)
        {
            cancelToken.ThrowIfCancellationRequested();

            try
            {
                await _globeStorage.Globe.UpdateAsync();
            }
            catch(Exception exception)
            {
                Debug.LogError(exception);
                return;
            }
            await _commandBlockerService.WaitBlockers();
        }
    }
}
