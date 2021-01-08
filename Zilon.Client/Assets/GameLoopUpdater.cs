using System;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;

class GameLoopUpdater
{
    [NotNull]
    private readonly IPlayer _player;

    [NotNull] 
    private readonly IAnimationBlockerService _commandBlockerService;

    [NotNull]
    private readonly IInventoryState _inventoryState;

    [NotNull]
    private readonly ISectorUiState _playerState;

    private CancellationTokenSource _cancellationTokenSource;

    public bool IsStarted { get; private set; }

    public GameLoopUpdater(
        IPlayer player,
        IAnimationBlockerService commandBlockerService,
        IInventoryState inventoryState,
        ISectorUiState playerState)
    {
        _commandBlockerService = commandBlockerService ?? throw new ArgumentNullException(nameof(commandBlockerService));
        _inventoryState = inventoryState ?? throw new ArgumentNullException(nameof(inventoryState));
        _playerState = playerState ?? throw new ArgumentNullException(nameof(playerState));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var cancelToken = _cancellationTokenSource.Token;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var updateTask = StartGameLoopUpdateAsync(cancelToken);

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

    private async Task StartGameLoopUpdateAsync(CancellationToken cancelToken)
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

            var animationBlockerTask = _commandBlockerService.WaitBlockersAsync();
            var fuseDelayTask = Task.Delay(10_000);

            await Task.WhenAny(animationBlockerTask, fuseDelayTask);
            _commandBlockerService.DropBlockers();
        }
    }

    private void ClearupActionUiState()
    {
        _inventoryState.SelectedProp = null;
        _playerState.SelectedViewModel = null;
    }
}
