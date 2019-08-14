using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании для человеческих персонажей.
    /// </summary>
    public sealed class HumanSurvivalData : ISurvivalData
    {
        private const int START_SURVIVAL_STAT = 150;
        private const int MIN_SURVIVAL_STAT = -150;
        private const int MAX_SURVIVAL_STAT = 300;
        private const int MAX_SURVIVAL_STAT_KEYPOINT = -100;
        private const int STRONG_SURVIVAL_STAT_KEYPOINT = -50;
        private const int LESSER_SURVIVAL_STAT_KEYPOINT = 0;
        private readonly IPersonScheme _personScheme;
        private readonly ISurvivalRandomSource _randomSource;

        public HumanSurvivalData([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource)
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));

            Stats = new[] {
                new SurvivalStat(_personScheme.Hp, 0, _personScheme.Hp){
                    Type = SurvivalStatType.Health
                },
                CreateStat(SurvivalStatType.Satiety),
                CreateStat(SurvivalStatType.Water)
            };
        }

        public HumanSurvivalData([NotNull] IPersonScheme personScheme,
            [NotNull] IEnumerable<SurvivalStat> stats,
            [NotNull] ISurvivalRandomSource randomSource)
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));

            Stats = stats.ToArray();
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

        /// <summary>Обновление состояния данных о выживании.</summary>
        public void Update()
        {
            foreach (var stat in Stats)
            {
                var roll = _randomSource.RollSurvival(stat);
                var successRoll = GetSuccessRoll();

                if (roll >= successRoll)
                {
                    ChangeStatInner(stat, -stat.Rate);
                }
            }
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
            var oldValue = stat.Value;

            stat.Value += value;

            if (stat.KeyPoints != null)
            {
                CheckStatKeyPoints(stat, oldValue);
            }

            ProcessIfHealth(stat);
        }

        private void CheckStatKeyPoints(SurvivalStat stat, int oldValue)
        {
            var crossedKeyPoints = stat.KeyPoints.CalcKeyPointsInRange(oldValue, stat.Value);

            if (crossedKeyPoints.Any())
            {
                DoStatCrossKeyPoint(stat, crossedKeyPoints);
            }
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

        private static SurvivalStat CreateStat(SurvivalStatType type)
        {
            var stat = new SurvivalStat(START_SURVIVAL_STAT, MIN_SURVIVAL_STAT, MAX_SURVIVAL_STAT)
            {
                Type = type,
                Rate = 1,
                KeyPoints = new[]{
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, MAX_SURVIVAL_STAT_KEYPOINT),
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, STRONG_SURVIVAL_STAT_KEYPOINT),
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, LESSER_SURVIVAL_STAT_KEYPOINT)
                    }
            };
            return stat;
        }

        private void DoStatCrossKeyPoint(SurvivalStat stat, IEnumerable<SurvivalStatKeyPoint> keyPoints)
        {
            var args = new SurvivalStatChangedEventArgs(stat, keyPoints);
            StatCrossKeyValue?.Invoke(this, args);
        }


        private static int GetSuccessRoll()
        {
            // В будущем этот порог будет расчитываться, исходя из характеристик, перков и экипировки персонажа.
            return 4;
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public void ResetStats()
        {
            Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health)?.ChangeStatRange(0, _personScheme.Hp);
        }
    }
}
