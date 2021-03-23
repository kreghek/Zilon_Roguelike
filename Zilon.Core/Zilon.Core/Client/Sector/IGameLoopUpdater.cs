using System;

namespace Zilon.Core.Client.Sector
{
    public interface IGameLoopUpdater
    {
        bool IsStarted { get; }

        void Start();
        void Stop();

        event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;
    }
}