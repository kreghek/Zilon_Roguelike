using System;

using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public class PersuitNearbyIntruderData: ILogicStateData
    {
        public PersuitNearbyIntruderData(IActor target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public IActor Target { get; }
    }
}
