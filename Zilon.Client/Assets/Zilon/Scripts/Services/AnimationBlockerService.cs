using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Zilon.Scripts.Services
{
    class AnimationBlockerService : IAnimationBlockerService
    {
        private readonly ConcurrentDictionary<ICommandBlocker, byte> _commandBlockers;

        private TaskCompletionSource<bool> tcs;

        private object _lockObject;

        public AnimationBlockerService()
        {
            _lockObject = new object();
            _commandBlockers = new ConcurrentDictionary<ICommandBlocker, byte>();
        }

        /// <inheritdoc/>
        public bool HasBlockers { get => _commandBlockers.Count > 0; }

        /// <inheritdoc/>
        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            lock (_lockObject)
            {
                commandBlocker.Released += CommandBlocker_Release;
                _commandBlockers.TryAdd(commandBlocker, 0);
                tcs = new TaskCompletionSource<bool>();
            }
        }

        /// <inheritdoc/>
        public void DropBlockers()
        {
            lock (_lockObject)
            {
                foreach (var commandBlocker in _commandBlockers.Keys.ToArray())
                {
                    commandBlocker.Released -= CommandBlocker_Release;
                }

                _commandBlockers.Clear();
            }
        }

        /// <inheritdoc/>
        public Task WaitBlockersAsync()
        {
            lock (_lockObject)
            {
                if (tcs is null)
                {
                    return Task.CompletedTask;
                }
                else
                {
                    return tcs.Task;
                }
            }
        }

        private void CommandBlocker_Release(object sender, System.EventArgs e)
        {
            lock (_lockObject)
            {
                var blocker = (ICommandBlocker)sender;
                blocker.Released -= CommandBlocker_Release;
                _commandBlockers.TryRemove(blocker, out var _);

                if (!HasBlockers)
                {
                    if (tcs != null)
                    {
                        tcs.SetResult(true);
                    }
                }
            }
        }
    }
}
