using System;

using JetBrains.Annotations;

namespace Zilon.Core.Commands
{
    [PublicAPI]
    public interface ICommandPool
    {
        bool IsEmpty { get; }
        ICommand? Pop();
        void Push(ICommand command);

        event EventHandler? CommandPushed;
    }
}