using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Zilon.Scripts.Services
{
    class CommandBlockerService : ICommandBlockerService
    {
        private readonly HashSet<ICommandBlocker> _commandBlockers;

        private TaskCompletionSource<bool> tcs;

        public CommandBlockerService()
        {
            _commandBlockers = new HashSet<ICommandBlocker>();
        }

        /// <inheritdoc/>
        public bool HasBlockers { get => _commandBlockers.Count > 0; }

        /// <inheritdoc/>
        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            commandBlocker.Released += CommandBlocker_Release;
            _commandBlockers.Add(commandBlocker);
            tcs = new TaskCompletionSource<bool>();
        }

        /// <inheritdoc/>
        public void DropBlockers()
        {
            foreach (var commandBlocker in _commandBlockers)
            {
                commandBlocker.Released -= CommandBlocker_Release;
            }

            _commandBlockers.Clear();
        }

        /// <inheritdoc/>
        public Task WaitBlockersAsync()
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

        private void CommandBlocker_Release(object sender, System.EventArgs e)
        {
            var blocker = (ICommandBlocker)sender;
            blocker.Released -= CommandBlocker_Release;
            _commandBlockers.Remove(blocker);

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
