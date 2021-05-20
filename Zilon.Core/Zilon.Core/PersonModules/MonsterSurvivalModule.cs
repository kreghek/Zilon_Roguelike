﻿using System;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация данных о выживании для монстров.
    /// </summary>
    public sealed class MonsterSurvivalModule : SurvivalModuleBase
    {
        public MonsterSurvivalModule([NotNull] IMonsterScheme monsterScheme) : base(GetStats(monsterScheme))
        {
            if (monsterScheme == null)
            {
                throw new ArgumentNullException(nameof(monsterScheme));
            }
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public override void ResetStats()
        {
            // эта реализация пока ничего не делает.
        }

        /// <summary>
        /// Обновление состояния данных о выживании.
        /// </summary>
        public override void Update()
        {
            // Монстры не требуют расчета своих характеристик.
        }

        private static SurvivalStat[] GetStats([NotNull] IMonsterScheme monsterScheme)
        {
            if (monsterScheme is null)
            {
                throw new ArgumentNullException(nameof(monsterScheme));
            }

            return new[]
            {
                new SurvivalStat(monsterScheme.Hp, 0, monsterScheme.Hp)
                {
                    Type = SurvivalStatType.Health
                }
            };
        }
    }
}