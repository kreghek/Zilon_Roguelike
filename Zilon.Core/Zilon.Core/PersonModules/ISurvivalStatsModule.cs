using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Данные персонажа по выживанию.
    /// </summary>
    /// <remarks>
    /// Здесь будут сведения о питании, отдыхе, ранах, эмоциональном состоянии персонажа.
    /// </remarks>
    public interface ISurvivalModule: IPersonModule
    {
        /// <summary>
        /// Текущие характеристики.
        /// </summary>
        SurvivalStat[] Stats { get; }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        void ResetStats();

        /// <summary>
        /// Обновление состояния данных о выживании.
        /// </summary>
        void Update();

        /// <summary>
        /// Снижение характеристики.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет произведено влияние. </param>
        /// <param name="value"> Значение, на которое снижается текущий запас. </param>
        void DecreaseStat(SurvivalStatType type, int value);

        /// <summary>
        /// Пополнение запаса характеристики.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет произведено влияние. </param>
        /// <param name="value"> Значение, на которое восстанавливается текущий запас. </param>
        void RestoreStat(SurvivalStatType type, int value);

        /// <summary>
        /// Форсированно установить запас здоровья.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет произведено влияние. </param>
        /// <param name="value"> Целевое значение запаса характеристики. </param>
        void SetStatForce(SurvivalStatType type, int value);

        /// <summary>
        /// Признак того, что персонаж мёртв.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Происходит, если персонаж умирает.
        /// </summary>
        event EventHandler Dead;

        /// <summary>
        /// Событие, которое происходит, если значение характеристики
        /// изменяется.
        /// </summary>
        event EventHandler<SurvivalStatChangedEventArgs> StatChanged;
    }

    public sealed class HumanSurvivalModule : SurvivalModuleBase
    {
        private readonly IPersonScheme _personScheme;
        private readonly ISurvivalRandomSource _randomSource;

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource) : base(GetStats(personScheme))
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));

            foreach (var stat in Stats)
            {
                stat.Changed += Stat_Changed;
            }
        }

        private static SurvivalStat[] GetStats([NotNull] IPersonScheme personScheme)
        {
            if (personScheme is null)
            {
                throw new ArgumentNullException(nameof(personScheme));
            }
            // Устанавливаем характеристики выживания персонажа
            var statList = new List<SurvivalStat>();
            SetHitPointsStat(personScheme, statList);

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

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Wound,
                    PersonSurvivalStatType.Wound,
                    statList);
            }

            CreateUselessStat(SurvivalStatType.Breath, statList);
            CreateUselessStat(SurvivalStatType.Energy, statList);

            return statList.ToArray();
        }

        private static void CreateUselessStat(SurvivalStatType statType, List<SurvivalStat> statList)
        {
            var stat = new SurvivalStat(100, 0, 100)
            {
                Type = statType,
                Rate = 1,
                DownPassRoll = 0
            };

            statList.Add(stat);
        }

        private void Stat_Changed(object sender, EventArgs e)
        {
            DoStatChanged((SurvivalStat)sender);
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

            statList.Add(hpStat);
        }

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] IEnumerable<SurvivalStat> stats,
            [NotNull] ISurvivalRandomSource randomSource) : base(stats.ToArray())
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        }

        /// <summary>Обновление состояния данных о выживании.</summary>
        public override void Update()
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

        private static SurvivalStat CreateStat(
            SurvivalStatType type,
            PersonSurvivalStatType schemeStatType,
            IPersonSurvivalStatSubScheme[] survivalStats)
        {
            var statScheme = survivalStats.SingleOrDefault(x => x.Type == schemeStatType);
            if (statScheme is null)
            {
                return null;
            }

            var keySegmentList = new List<SurvivalStatKeySegment>();
            if (statScheme.KeyPoints != null)
            {
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Max, PersonSurvivalStatKeypointLevel.Max, statScheme.KeyPoints, keySegmentList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Strong, PersonSurvivalStatKeypointLevel.Strong, statScheme.KeyPoints, keySegmentList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Lesser, PersonSurvivalStatKeypointLevel.Lesser, statScheme.KeyPoints, keySegmentList);
            }

            var stat = new SurvivalStat(statScheme.StartValue, statScheme.MinValue, statScheme.MaxValue)
            {
                Type = type,
                Rate = 1,
                KeySegments = keySegmentList.ToArray(),
                DownPassRoll = statScheme.DownPassRoll.GetValueOrDefault(SurvivalStat.DEFAULT_DOWN_PASS_VALUE)
            };

            return stat;
        }

        private static void AddKeyPointFromScheme(
            SurvivalStatHazardLevel segmentLevel,
            PersonSurvivalStatKeypointLevel schemeSegmentLevel,
            IPersonSurvivalStatKeySegmentSubScheme[] keyPoints,
            List<SurvivalStatKeySegment> keyPointList)
        {
            var schemeKeySegment = GetKeyPointSchemeValue(schemeSegmentLevel, keyPoints);
            if (schemeKeySegment == null)
            {
                return;
            }

            var keySegment = new SurvivalStatKeySegment(schemeKeySegment.Start, schemeKeySegment.End, segmentLevel);
            keyPointList.Add(keySegment);
        }

        private void DoStatChanged(SurvivalStat stat)
        {
            var args = new SurvivalStatChangedEventArgs(stat);
            InvokeStatChangedEvent(this, args);
        }

        private int GetStatDownRoll(SurvivalStat stat)
        {
            return stat.DownPassRoll;
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public override void ResetStats()
        {
            Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health)?.ChangeStatRange(0, _personScheme.Hp);

            foreach (var stat in Stats)
            {
                stat.DownPassRoll = SurvivalStat.DEFAULT_DOWN_PASS_VALUE;
            }
        }

        private static IPersonSurvivalStatKeySegmentSubScheme GetKeyPointSchemeValue(
            PersonSurvivalStatKeypointLevel level,
            IPersonSurvivalStatKeySegmentSubScheme[] keyPoints)
        {
            return keyPoints.SingleOrDefault(x => x.Level == level);
        }
    }

    public abstract class SurvivalModuleBase : ISurvivalModule
    {
        protected SurvivalModuleBase(SurvivalStat[] stats)
        {
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
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

        protected void ChangeStatInner(SurvivalStat stat, int value)
        {
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
        public override void Update()
        {
            // Монстры не требуют расчета своих характеристик.
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public override void ResetStats()
        {
            // эта реализация пока ничего не делает.
        }
    }
}
