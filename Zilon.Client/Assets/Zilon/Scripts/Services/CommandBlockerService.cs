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

        public bool HasBlockers { get => _commandBlockers.Count > 0; }

        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            commandBlocker.Released += CommandBlocker_Release;
            _commandBlockers.Add(commandBlocker);
            tcs = new TaskCompletionSource<bool>();
        }

        public void DropBlockers()
        {
            _commandBlockers.Clear();
        }

        public Task WaitBlockers()
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
