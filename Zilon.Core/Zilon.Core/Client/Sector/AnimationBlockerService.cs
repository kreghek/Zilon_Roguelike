using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public class AnimationBlockerService : IAnimationBlockerService
    {
        private readonly ConcurrentDictionary<ICommandBlocker, byte> _commandBlockers;

        private readonly object _lockObject;

        private TaskCompletionSource<bool>? _tcs;

        public AnimationBlockerService()
        {
            _lockObject = new object();
            _commandBlockers = new ConcurrentDictionary<ICommandBlocker, byte>();
        }

        private void CommandBlocker_Release(object? sender, EventArgs e)
        {
            Console.WriteLine($"Blocker {sender} released.");

            if (sender is null)
            {
                throw new InvalidOperationException("Unexpectible event.");
            }

            lock (_lockObject)
            {
                var blocker = (ICommandBlocker)sender;
                blocker.Released -= CommandBlocker_Release;
                _commandBlockers.TryRemove(blocker, out var _);

                if (!HasBlockers && _tcs != null)
                {
                    var tempTcs = _tcs;
                    _tcs = null;
                    tempTcs.SetResult(true);
                }
            }
        }

        /// <inheritdoc />
        public bool HasBlockers => !_commandBlockers.IsEmpty;

        /// <inheritdoc />
        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            Console.WriteLine($"Add blocker {commandBlocker}.");
            lock (_lockObject)
            {
                commandBlocker.Released += CommandBlocker_Release;
                _commandBlockers.TryAdd(commandBlocker, 0);

                if (_tcs is null)
                {
                    _tcs = new TaskCompletionSource<bool>();
                }
            }
        }

        /// <inheritdoc />
        public void DropBlockers()
        {
            Console.WriteLine("Drop blockers.");
            lock (_lockObject)
            {
                foreach (var commandBlocker in _commandBlockers.Keys.ToArray())
                {
                    commandBlocker.Released -= CommandBlocker_Release;
                }

                _commandBlockers.Clear();
                if (_tcs != null && _tcs.TrySetResult(true))
                {
                }

                _tcs = null;
            }
        }

        /// <inheritdoc />
        public Task WaitBlockersAsync()
        {
            lock (_lockObject)
            {
                if (_tcs is null)
                {
                    Console.WriteLine("Wait blockers (null).");
                    return Task.CompletedTask;
                }

                Console.WriteLine("Wait blockers.");
                return _tcs.Task;
            }
        }
    }
}