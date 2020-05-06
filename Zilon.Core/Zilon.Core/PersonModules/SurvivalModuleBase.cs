using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;

namespace Zilon.Core.PersonModules
{
    public abstract class SurvivalModuleBase : ISurvivalModule
    {
        protected SurvivalModuleBase(IEnumerable<SurvivalStat> stats)
        {
            Stats = stats?.ToArray() ?? throw new ArgumentNullException(nameof(stats));
            IsActive = true;
        }

        /// <summary>Текущие характеристики.</summary>
        public SurvivalStat[] Stats { get; }

        /// <summary>
        /// Событие, которое происходит, если значение характеристики
        /// пересекает ключевое значение (мин/макс/четверти/0).
        /// </summary>
        public event EventHandler<SurvivalStatChangedEventArgs> StatChanged;

        /// <summary>Признак того, что персонаж мёртв.</summary>
        public bool IsDead { get; private set; }
        public string Key { get => nameof(ISurvivalModule); }
        public bool IsActive { get; set; }

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
                if (stat.Value < stat.Range.Max)
                {
                    ChangeStatInner(stat, value);
                }
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
                if (stat.Value > stat.Range.Min)
                {
                    ChangeStatInner(stat, -value);
                }
            }
        }

        /// <summary>
        /// Invokes the stat changed event with the given parameters
        /// </summary>
        /// <param name="caller">The object performing the call</param>
        /// <param name="args">The <see cref="SurvivalStatChangedEventArgs"/> instance containing the event data.</param>
        public void InvokeStatChangedEvent(SurvivalModuleBase caller, SurvivalStatChangedEventArgs args)
        {
            StatChanged?.Invoke(caller, args);
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

        private static void ValidateStatChangeValue(int value)
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

        protected void ChangeStatInner(SurvivalStat stat, int value)
        {
            if (stat is null)
            {
                throw new ArgumentNullException(nameof(stat));
            }

            stat.Value += value;

            ProcessIfHealth(stat, value);
            ProcessIfWound(stat);
        }

        /// <summary>
        /// Индивидуально обрабатывает характеристику, если это здоровье.
        /// </summary>
        /// <param name="stat"> Обрабатываемая характеристика. </param>
        private void ProcessIfHealth(SurvivalStat stat, int value)
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
            else
            {
                if (value < 0)
                {
                    SetWoundCounter();
                }
            }
        }

        /// <summary>
        /// Если персонаж получает урон, то выставляетс счётчик раны.
        /// Когда счтётчик раны снижается до 0, восстанавливается 1 здоровья.
        /// </summary>
        private void SetWoundCounter()
        {
            var woundStat = Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Wound);
            if (woundStat != null)
            {
                SetStatForce(SurvivalStatType.Wound, woundStat.Range.Max);
            }
        }

        private void ProcessIfWound(SurvivalStat stat)
        {
            if (stat.Type != SurvivalStatType.Wound)
            {
                return;
            }

            var woundCounter = stat.Value;
            if (woundCounter <= 0)
            {
                // Если счётчик раны дошёл до 0,
                // то восстанавливаем единицу здоровья.

                RestoreStat(SurvivalStatType.Health, 1);
            }
        }

        private void DoDead()
        {
            Dead?.Invoke(this, new EventArgs());
        }

        public abstract void ResetStats();
        public abstract void Update();
    }
}
