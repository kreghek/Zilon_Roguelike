using System;

using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics
{
    public sealed class TransitionUsedEventArgs : EventArgs
    {
        public TransitionUsedEventArgs(IActor actor, SectorTransition transition)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }

        public IActor Actor { get; }
        public SectorTransition Transition { get; }
    }
}