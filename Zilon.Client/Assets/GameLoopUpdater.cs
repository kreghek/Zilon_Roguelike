using System;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;

class GameLoopUpdater
{
    [NotNull]
    private readonly IPlayer _player;

    [NotNull] 
    private readonly IAnimationBlockerService _commandBlockerService;

    [NotNull] [Inject] private readonly IInventoryState _inventoryState;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    private CancellationTokenSource _cancellationTokenSource;

    public bool IsStarted { get; private set; }

    public GameLoopUpdater(IPlayer player, IAnimationBlockerService commandBlockerService)
    {
        _commandBlockerService = commandBlockerService ?? throw new ArgumentNullException(nameof(commandBlockerService));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var cancelToken = _cancellationTokenSource.Token;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var updateTask = StartGameLoopUpdate(cancelToken);

        updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnFaulted);
        updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnCanceled);
        updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnRanToCompletion);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        IsStarted = true;
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task StartGameLoopUpdate(CancellationToken cancelToken)
    {
        while (!_player.MainPerson.GetModule<ISurvivalModule>().IsDead)
        {
            cancelToken.ThrowIfCancellationRequested();

            try
            {
                await _player.Globe.UpdateAsync();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                return;
            }

            ClearupActionUiState();

            await _commandBlockerService.WaitBlockersAsync();
        }
    }

    private void ClearupActionUiState()
    {
        _inventoryState.SelectedProp = null;
        _playerState.SelectedViewModel = null;
    }
}
