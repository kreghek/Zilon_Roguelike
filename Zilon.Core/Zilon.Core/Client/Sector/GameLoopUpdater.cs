using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Zilon.Core.Client.Sector
{
    public class GameLoopUpdater : IDisposable, IGameLoopUpdater
    {
        [NotNull] private readonly IAnimationBlockerService _animationBlockerService;

        [NotNull] private readonly IGameLoopContext _gameLoopContext;

        private CancellationTokenSource? _cancellationTokenSource;

        public GameLoopUpdater(
            IGameLoopContext gameLoopContext,
            IAnimationBlockerService animationBlockerService)
        {
            _gameLoopContext = gameLoopContext ?? throw new ArgumentNullException(nameof(gameLoopContext));
            _animationBlockerService = animationBlockerService ??
                                       throw new ArgumentNullException(nameof(animationBlockerService));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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

        public void Stop()
        {
            if (_cancellationTokenSource is null)
            {
                // Means Start was not called.
            }
            else
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}