using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public sealed class GlobeLoopUpdater : IDisposable, IGlobeLoopUpdater
    {
        private readonly IAnimationBlockerService _animationBlockerService;

        private readonly IGlobeLoopContext _gameLoopContext;

        private CancellationTokenSource? _cancellationTokenSource;

        [ExcludeFromCodeCoverage]
        public GlobeLoopUpdater(
            IGlobeLoopContext gameLoopContext,
            IAnimationBlockerService animationBlockerService)
        {
            _gameLoopContext = gameLoopContext;
            _animationBlockerService = animationBlockerService;
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
                    ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                }

                var animationBlockerTask = _animationBlockerService.WaitBlockersAsync();
                var fuseDelayTask = Task.Delay(10_000, cancelToken);

                await Task.WhenAny(animationBlockerTask, fuseDelayTask).ConfigureAwait(false);
                _animationBlockerService.DropBlockers();
            }
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
            }
        }

        public bool IsStarted { get; private set; }

        public event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var cancelToken = _cancellationTokenSource.Token;

            var updateTask = StartGameLoopUpdateAsync(cancelToken);

            updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnFaulted);
            updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnCanceled);
            updateTask.ContinueWith(task => IsStarted = false, TaskContinuationOptions.OnlyOnRanToCompletion);

            IsStarted = true;
        }

        [ExcludeFromCodeCoverage]
        public void Stop()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}