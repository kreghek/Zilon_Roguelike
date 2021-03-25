using System;

namespace Zilon.Core.Client.Sector
{
    public interface IGlobeLoopUpdater
    {
        bool IsStarted { get; }

        void Start();
        void Stop();

        event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;
    }
}