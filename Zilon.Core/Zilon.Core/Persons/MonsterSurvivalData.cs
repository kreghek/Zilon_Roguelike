using System;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании для монстров.
    /// </summary>
    public sealed class MonsterSurvivalData : ISurvivalData
    {
        public MonsterSurvivalData([NotNull] IMonsterScheme monsterScheme)
        {
            if (monsterScheme == null)
            {
                throw new ArgumentNullException(nameof(monsterScheme));
            }

            Stats = new[] {
               new SurvivalStat(monsterScheme.Hp, 0, monsterScheme.Hp){
                    Type = SurvivalStatType.Health
                }
            };
        }

        /// <summary>Текущие характеристики.</summary>
        public SurvivalStat[] Stats { get; }

        /// <summary>Признак того, что персонаж мёртв.</summary>
        public bool IsDead { get; private set; }

        /// <summary>
        /// Событие, которое происходит, если значение характеристики
        /// пересекает ключевое значение (мин/макс/четверти/0).
        /// </summary>
        public event EventHandler<SurvivalStatChangedEventArgs> StatCrossKeyValue;

        /// <summary>Происходит, если персонаж умирает.</summary>
        public event EventHandler Dead;

        /// <summary>Пополнение запаса характеристики.</summary>
        /// <param name="type">Тип характеритсики, которая будет произведено влияние.</param>
        /// <param name="value">Значение, на которое восстанавливается текущий запас.</param>
        public void RestoreStat(SurvivalStatType type, int value)
        {
            ValidateStatChangeValue(value);

            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                ChangeStatInner(stat, value);
            }
        }

        /// <summary>Снижение характеристики.</summary>
        /// <param name="type">Тип характеритсики, которая будет произведено влияние.</param>
        /// <param name="value">Значение, на которое снижается текущий запас.</param>
        public void DecreaseStat(SurvivalStatType type, int value)
        {
            ValidateStatChangeValue(value);

            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                ChangeStatInner(stat, -value);
            }
        }

        /// <summary>Форсированно установить запас здоровья.</summary>
        /// <param name="type">Тип характеритсики, которая будет произведено влияние.</param>
        /// <param name="value">Целевое значение запаса характеристики.</param>
        public void SetStatForce(SurvivalStatType type, int value)
        {
            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                stat.Value = value;
            }
        }

        /// <summary>
        /// Обновление состояния данных о выживании.
        /// </summary>
        public void Update()
        {
            // Монстры не требуют расчета своих характеристик.
        }

        private void ValidateStatChangeValue(int value)
        {
            if (value == 0)
            {
                return;
            }

            if (value < 0)
            {
                throw new ArgumentException("Величина не может быть меньше 0.", nameof(value));
            }
        }

        private void ChangeStatInner(SurvivalStat stat, int value)
        {
            stat.Value += value;

            ProcessIfHealth(stat);
        }

        /// <summary>
        /// Индивидуально обрабатывает характеристику, если это здоровье.
        /// </summary>
        /// <param name="stat"> Обрабатываемая характеристика. </param>
        private void ProcessIfHealth(SurvivalStat stat)
        {
            if (stat.Type != SurvivalStatType.Health)
            {
                return;
            }

            var hp = stat.Value;
            if (hp <= 0)
            {
                IsDead = true;
                DoDead();
            }
        }

        private void DoDead()
        {
            Dead?.Invoke(this, new EventArgs());
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public void ResetStats()
        {
            // эта реализация пока ничего не делает.
        }
    }
}
