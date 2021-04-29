using System;

using JetBrains.Annotations;

namespace Zilon.Core.Commands
{
    [PublicAPI]
    public interface ICommandPool
    {
        ICommand? Pop();
        void Push(ICommand command);

        event EventHandler? CommandPushed;
    }
}