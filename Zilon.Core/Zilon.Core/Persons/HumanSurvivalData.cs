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
        private readonly IPersonScheme _personScheme;
        private readonly ISurvivalRandomSource _randomSource;

        public HumanSurvivalData([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource)
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));

            // Устанавливаем характеристики выживания персонажа
            var statList = new List<SurvivalStat>();
            SetHitPointsStat(_personScheme, statList);

            // Выставляем сытость/упоённость
            if (personScheme.SurvivalStats != null)
            {
                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Satiety,
                    PersonSurvivalStatType.Satiety,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Hydration,
                    PersonSurvivalStatType.Hydration,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Intoxication,
                    PersonSurvivalStatType.Intoxication,
                    statList);
            }

            Stats = statList.ToArray();
        }

        private static void CreateStatFromScheme(IPersonSurvivalStatSubScheme[] survivalStats,
            SurvivalStatType statType,
            PersonSurvivalStatType schemeStatType,
            List<SurvivalStat> statList)
        {
            var stat = CreateStat(statType, schemeStatType, survivalStats);
            if (stat != null)
            {
                statList.Add(stat);
            }
        }

        private static void SetHitPointsStat(IPersonScheme personScheme, IList<SurvivalStat> statList)
        {
            var hpStat = new HpSurvivalStat(personScheme.Hp, 0, personScheme.Hp)
            {
                Type = SurvivalStatType.Health
            };

            // Размечаем хп ключевыми точками


            statList.Add(hpStat);
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
                if (stat.Rate == 0)
                {
                    continue;
                }

                var roll = _randomSource.RollSurvival(stat);
                var statDownRoll = GetStatDownRoll(stat);

                if (roll < statDownRoll)
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

        private static SurvivalStat CreateStat(SurvivalStatType type, PersonSurvivalStatType schemeStatType, IPersonSurvivalStatSubScheme[] survivalStats)
        {
            var statScheme = survivalStats.SingleOrDefault(x => x.Type == schemeStatType);
            if (statScheme == null)
            {
                return null;
            }

            var keyPointList = new List<SurvivalStatKeyPoint>();
            if (statScheme.KeyPoints != null)
            {
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Max, PersonSurvivalStatKeypointLevel.Max, statScheme.KeyPoints, keyPointList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Strong, PersonSurvivalStatKeypointLevel.Strong, statScheme.KeyPoints, keyPointList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Lesser, PersonSurvivalStatKeypointLevel.Lesser, statScheme.KeyPoints, keyPointList);

                //Ниже пока не актуально. Алгоритм работает так, что ему не важен порядок ключевых точек.
                //// По условиям работы с о схемами, в схемах ключевые значения
                //// могут быть в любом порядке.
                //// При создании ключевых точек их нужно сортировать по возрастанию, чтобы корректно
                //// обрабатываться пересечение ключевых точек.
                //keyPointList.Sort((a, b) => a.Value.CompareTo(b.Value));
            }

            var stat = new SurvivalStat(statScheme.StartValue, statScheme.MinValue, statScheme.MaxValue)
            {
                Type = type,
                Rate = 1,
                KeyPoints = keyPointList.ToArray()
            };

            return stat;
        }

        private static void AddKeyPointFromScheme(
            SurvivalStatHazardLevel max1,
            PersonSurvivalStatKeypointLevel max2,
            IPersonSurvivalStatKeyPointSubScheme[] keyPoints,
            List<SurvivalStatKeyPoint> keyPointList)
        {
            var schemeKeyPoint = GetKeyPointSchemeValue(max2, keyPoints);
            if (schemeKeyPoint == null)
            {
                return;
            }

            var keyPoint = new SurvivalStatKeyPoint(max1, schemeKeyPoint.Value);
            keyPointList.Add(keyPoint);
        }

        private void DoStatCrossKeyPoint(SurvivalStat stat, SurvivalStatKeyPoint[] keyPoints)
        {
            var args = new SurvivalStatChangedEventArgs(stat, keyPoints);
            StatCrossKeyValue?.Invoke(this, args);
        }


        private int GetStatDownRoll(SurvivalStat stat)
        {
            return stat.DownPassRoll;
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public void ResetStats()
        {
            Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health)?.ChangeStatRange(0, _personScheme.Hp);

            foreach (var stat in Stats)
            {
                stat.DownPassRoll = SurvivalStat.DEFAULT_DOWN_PASS_VALUE;
            }
        }

        private static int? GetKeyPointSchemeValue(PersonSurvivalStatKeypointLevel level, IPersonSurvivalStatKeyPointSubScheme[] keyPoints)
        {
            return keyPoints.SingleOrDefault(x => x.Level == level)?.Value;
        }
    }
}
