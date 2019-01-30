﻿using System.Collections.Generic;

namespace Assets.Zilon.Scripts.Services
{
    class CommandBlockerService : ICommandBlockerService
    {
        private readonly HashSet<ICommandBlocker> _commandBlockers;

        public CommandBlockerService()
        {
            _commandBlockers = new HashSet<ICommandBlocker>();
        }

        public bool HasBlockers { get => _commandBlockers.Count > 0; }

        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            commandBlocker.Released += CommandBlocker_Release;
            _commandBlockers.Add(commandBlocker);
        }

        public void DropBlockers()
        {
            _commandBlockers.Clear();
        }

        private void CommandBlocker_Release(object sender, System.EventArgs e)
        {
            var blocker = (ICommandBlocker)sender;
            blocker.Released -= CommandBlocker_Release;
            _commandBlockers.Remove(blocker);
        }
    }
}
