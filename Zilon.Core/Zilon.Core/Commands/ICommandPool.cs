using System;

namespace Zilon.Core.Commands
{
    public interface ICommandPool
    {
        /// <summary>
        /// Indicates the pool is empty.
        /// </summary>
        /// <remarks>
        /// Useful to detect the user can send next command in pool after execution of sequence of previous commands.
        /// </remarks>
        bool IsEmpty { get; }

        ICommand? Pop();

        void Push(ICommand command);

        event EventHandler? CommandPushed;
    }
}