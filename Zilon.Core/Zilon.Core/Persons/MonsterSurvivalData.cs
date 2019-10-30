using System;
using JetBrains.Annotations;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании для монстров.
    /// </summary>
    public sealed class MonsterSurvivalData : SurvivalDataBase, ISurvivalData
    {
        public MonsterSurvivalData([NotNull] IMonsterScheme monsterScheme) : base(GetStats(monsterScheme))
        {
            if (monsterScheme == null)
            {
                throw new ArgumentNullException(nameof(monsterScheme));
            }
        }

        private static SurvivalStat[] GetStats([NotNull] IMonsterScheme monsterScheme)
        {
            return new[] {
                new SurvivalStat(monsterScheme.Hp, 0, monsterScheme.Hp){
                    Type = SurvivalStatType.Health
                }
            };
        }
        
        /// <summary>
        /// Обновление состояния данных о выживании.
        /// </summary>
        public void Update()
        {
            // Монстры не требуют расчета своих характеристик.
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public void ResetStats()
        {
            // эта реализация пока ничего не делает.
        }
    }
}
