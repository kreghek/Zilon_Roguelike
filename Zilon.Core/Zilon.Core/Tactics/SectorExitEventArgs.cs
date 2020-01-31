using System;

namespace Zilon.Core.Tactics
{
    public sealed class SectorExitEventArgs: EventArgs
    {
        public SectorExitEventArgs(SectorTransition transition)
        {
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }

        public SectorTransition Transition { get; }
    }
}
