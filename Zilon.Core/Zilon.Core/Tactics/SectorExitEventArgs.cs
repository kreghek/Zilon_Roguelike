using System;

using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics
{
    public sealed class SectorExitEventArgs : EventArgs
    {
        public SectorExitEventArgs(RoomTransition transition)
        {
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }

        public RoomTransition Transition { get; }
    }
}