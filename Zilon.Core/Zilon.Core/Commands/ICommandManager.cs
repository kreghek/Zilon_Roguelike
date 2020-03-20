using System;

using JetBrains.Annotations;

namespace Zilon.Core.Commands
{
    [PublicAPI]
    public interface ICommandManager<TContext>
    {
        void Push(ICommand<TContext> command);
        ICommand<TContext> Pop();

        event EventHandler CommandPushed;
    }
}
