using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public interface ICommandLoopUpdater
    {
        bool IsStarted { get; }

        bool HasPendingCommands();

        Task StartAsync(CancellationToken cancellationToken);
        event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;

        event EventHandler? CommandAutoExecuted;

        event EventHandler? CommandProcessed;
    }
}