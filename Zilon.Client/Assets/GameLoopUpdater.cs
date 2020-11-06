using System;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

class GameLoopUpdater
{
    [NotNull] private readonly GlobeStorage _globeStorage;
    [NotNull] private readonly ICommandBlockerService _commandBlockerService;

    private CancellationTokenSource _cancellationTokenSource;

    public bool IsStarted { get; private set; }

    public GameLoopUpdater(GlobeStorage globeStorage, ICommandBlockerService commandBlockerService)
    {
        _globeStorage = globeStorage ?? throw new ArgumentNullException(nameof(globeStorage));
        _commandBlockerService = commandBlockerService ?? throw new ArgumentNullException(nameof(commandBlockerService));
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var cancelToken = _cancellationTokenSource.Token;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var updateTask = StartGameLoopUpdate(cancelToken);

        updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnFaulted);
        updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnCanceled);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        IsStarted = true;
    }

    public void Stop()
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
