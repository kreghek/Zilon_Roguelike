using System;

namespace Zilon.Core.Client.Sector
{
    public interface IGameLoopUpdater
    {
        bool IsStarted { get; }

        event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;

        void Start();
        void Stop();
    }
}