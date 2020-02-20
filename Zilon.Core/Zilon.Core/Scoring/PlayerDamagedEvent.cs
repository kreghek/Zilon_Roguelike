using System;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    public sealed class PlayerDamagedEvent : IPlayerEvent
    {
        public PlayerDamagedEvent(ITacticalAct tacticalAct, IActor damager)
        {
            TacticalAct = tacticalAct ?? throw new ArgumentNullException(nameof(tacticalAct));
            Damager = damager ?? throw new ArgumentNullException(nameof(damager));
        }

        public ITacticalAct TacticalAct { get; }

        public IActor Damager { get; }
    }
}
