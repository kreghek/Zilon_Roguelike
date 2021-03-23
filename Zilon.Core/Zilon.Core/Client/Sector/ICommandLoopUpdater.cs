using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public interface ICommandLoopUpdater
    {
        event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;

        event EventHandler? CommandAutoExecuted;

        event EventHandler? CommandProcessed;
        
        bool HasPendingCommands();
        
        Task StartAsync(CancellationToken cancellationToken);

        bool IsStarted { get; }
    }
}