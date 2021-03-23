using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    internal interface ICommandLoopUpdater
    {
        event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;

        bool HasPendingCommands();
        Task StartAsync(CancellationToken cancellationToken);
    }
}