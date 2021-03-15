
using System;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

namespace Assets.Zilon.Scripts.Models.Sector
{
    public class GameLoopUpdater
    {
        [NotNull]
        private readonly IGameLoopContext _gameLoopContext;

        [NotNull]
        private readonly IAnimationBlockerService _animationBlockerService;

        private CancellationTokenSource _cancellationTokenSource;

        public bool IsStarted { get; private set; }

        public GameLoopUpdater(
            IGameLoopContext gameLoopContext,
            IAnimationBlockerService animationBlockerService)
        {
            _gameLoopContext = gameLoopContext ?? throw new ArgumentNullException(nameof(gameLoopContext));
            _animationBlockerService = animationBlockerService ?? throw new ArgumentNullException(nameof(animationBlockerService));
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
            while (_gameLoopContext.HasNextIteration)
            {
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    await _gameLoopContext.UpdateAsync(cancelToken);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                    return;
                }

                var animationBlockerTask = _animationBlockerService.WaitBlockersAsync();
                var fuseDelayTask = Task.Delay(10_000);

                await Task.WhenAny(animationBlockerTask, fuseDelayTask).ConfigureAwait(false);
                _animationBlockerService.DropBlockers();
            }
        }
    }
}