using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public class AnimationBlockerService : IAnimationBlockerService
    {
        private readonly ConcurrentDictionary<ICommandBlocker, byte> _commandBlockers;

        private TaskCompletionSource<bool> _tcs;

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
                _tcs = new TaskCompletionSource<bool>();
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
                if (_tcs is null)
                {
                    return Task.CompletedTask;
                }
                else
                {
                    return _tcs.Task;
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
                    if (_tcs != null)
                    {
                        _tcs.SetResult(true);
                    }
                }
            }
        }
    }
}
