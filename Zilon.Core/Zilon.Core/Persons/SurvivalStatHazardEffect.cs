using System;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect, IActorStateEffect, ICombatStatEffect
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
            if (Level == SurvivalStatHazardLevel.Max)
            {
                actorState.TakeDamage(5);
            }
        }

        public void ApplyEveryTurn(ICombatStats combatStats)
        {
            // Этот эффект не влияет на характерстики каждый ход.
        }

        public void ApplyOnce(ICombatStats combatStats)
        {
            var q = 1f;
            switch (Level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    q = 0.9f;
                    break;

                case SurvivalStatHazardLevel.Strong:
                    q = 0.7f;
                    break;

                case SurvivalStatHazardLevel.Max:
                    q = 0.5f;
                    break;

                default:
                    throw new NotSupportedException("Неизветный уровень угрозы выживания.");
            }

            foreach (var item in combatStats.Stats)
            {
                item.Value = q * item.Value;
            }
        }

        public void Update()
        {
            // На персонажа нет влияния
        }
    }
}
