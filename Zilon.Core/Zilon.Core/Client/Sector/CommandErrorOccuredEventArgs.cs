using System;

using Zilon.Core.Commands;

namespace Zilon.Core.Client.Sector
{
    public sealed class CommandErrorOccuredEventArgs : ErrorOccuredEventArgs
    {
        public CommandErrorOccuredEventArgs(ICommand command, Exception exception) : base(exception)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        public ICommand Command { get; }
    }
}