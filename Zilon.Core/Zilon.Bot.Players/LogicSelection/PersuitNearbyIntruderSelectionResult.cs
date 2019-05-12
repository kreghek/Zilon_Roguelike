using System;

using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.LogicSelection
{
    public class PersuitNearbyIntruderSelectionResult: ILogicStateSelectorResult
    {
        public PersuitNearbyIntruderSelectionResult(IActor target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public IActor Target { get; }
    }
}
