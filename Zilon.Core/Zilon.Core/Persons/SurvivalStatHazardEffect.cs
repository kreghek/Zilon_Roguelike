using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect, IActorStateEffect
    {
        public SurvivalStatHazardEffect(SurvivalStatType type, SurvivalStatHazardLevel level)
        {
            Type = type;
            Level = level;
        }

        public SurvivalStatType Type { get; }

        public SurvivalStatHazardLevel Level { get; set; }

        public void Apply(IActorState actorState)
        {
            actorState.TakeDamage(5);
        }

        public void Update()
        {
            // На персонажа нет влияния
        }
    }
}
