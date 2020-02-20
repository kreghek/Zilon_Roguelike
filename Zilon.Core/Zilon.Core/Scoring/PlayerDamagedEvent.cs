using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    public sealed class PlayerDamagedEvent : IPlayerEvent
    {
        public PlayerDamagedEvent(ITacticalActScheme tacticalActScheme, IActor damager)
        {
            TacticalActScheme = tacticalActScheme ?? throw new ArgumentNullException(nameof(tacticalActScheme));
            Damager = damager ?? throw new ArgumentNullException(nameof(damager));
        }

        public ITacticalActScheme TacticalActScheme { get; }

        public IActor Damager { get; }
    }
}
