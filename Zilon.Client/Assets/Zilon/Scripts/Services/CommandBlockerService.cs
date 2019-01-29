using System;
using System.Collections.Generic;

namespace Assets.Zilon.Scripts.Services
{
    class CommandBlockerService : ICommandBlockerService
    {
        private readonly HashSet<ICommandBlocker> _commandBlockers;

        public CommandBlockerService()
        {
            _commandBlockers = new HashSet<ICommandBlocker>();
        }

        public bool HasBlockers { get; }

        public void AddBlocker(ICommandBlocker commandBlocker)
        {
            throw new NotImplementedException();
        }
    }
}
