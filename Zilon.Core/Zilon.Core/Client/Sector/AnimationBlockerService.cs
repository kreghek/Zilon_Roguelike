using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public class AnimationBlockerService : IAnimationBlockerService
    {
        private const int SEMAPHORE_WAIT_TIMEOUT_MILLISECONDS = 10000;

        private readonly ConcurrentDictionary<ICommandBlocker, byte> _commandBlockers;
        private readonly SemaphoreSlim _semaphore;

        private TaskCompletionSource<bool>? _tcs;

        public AnimationBlockerService()
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _commandBlockers = new ConcurrentDictionary<ICommandBlocker, byte>();
        }

        private async void CommandBlocker_Release(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                throw new InvalidOperationException("Unexpectible event sender. It must not be null.");
            }

            await _semaphore.WaitAsync(SEMAPHORE_WAIT_TIMEOUT_MILLISECONDS).ConfigureAwait(false);

            var blocker = (ICommandBlocker)sender;
            blocker.Released -= CommandBlocker_Release;
            _commandBlockers.TryRemove(blocker, out var _);

            if (!HasBlockers && _tcs != null)
            {
                _tcs.SetResult(true);
                _tcs = null;
            }

            _semaphore.Release();
        }

        /// <inheritdoc />
        public bool HasBlockers => !_commandBlockers.IsEmpty;

        /// <inheritdoc />
        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            _semaphore.Wait(SEMAPHORE_WAIT_TIMEOUT_MILLISECONDS);

            commandBlocker.Released += CommandBlocker_Release;
            _commandBlockers.TryAdd(commandBlocker, 0);

            if (_tcs is null)
            {
                // Use TaskCreationOptions.RunContinuationsAsynchronously to prevent deadlocks.
                // This is best practice if there are no strong reason to avoid it option.
                // https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#always-create-taskcompletionsourcet-with-taskcreationoptionsruncontinuationsasynchronously
                _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            _semaphore.Release();
        }

        /// <inheritdoc />
        public void DropBlockers()
        {
            _semaphore.Wait(SEMAPHORE_WAIT_TIMEOUT_MILLISECONDS);

            foreach (var commandBlocker in _commandBlockers.Keys.ToArray())
            {
                commandBlocker.Released -= CommandBlocker_Release;
            }

            _commandBlockers.Clear();
            if (_tcs != null)
            {
                _tcs.TrySetResult(true);
            }

            _tcs = null;

            _semaphore.Release();
        }

        /// <inheritdoc />
        public async Task WaitBlockersAsync()
        {
            await _semaphore.WaitAsync(SEMAPHORE_WAIT_TIMEOUT_MILLISECONDS).ConfigureAwait(false);

            if (_tcs is null)
            {
                return;
            }

            var waitTask = _tcs.Task;

            _semaphore.Release();

            await waitTask.ConfigureAwait(false);
        }
    }
}