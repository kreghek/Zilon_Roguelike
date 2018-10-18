using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании.
    /// </summary>
    public sealed class SurvivalData : ISurvivalData
    {
        public SurvivalData(SurvivalStat[] stats)
        {
            Stats = stats;
        }

        public SurvivalStat[] Stats { get; }
        public bool IsDead { get; private set; }

        public event EventHandler<SurvivalStatChangedEventArgs> StatCrossKeyValue;
        public event EventHandler Dead;

        public void RestoreStat(SurvivalStatType type, int value)
        {
            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                ChangeStatInner(stat, value);
            }
        }

        public void DecreaseStat(SurvivalStatType type, int value)
        {
            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                ChangeStatInner(stat, -value);
            }
        }

        public void SetStatForce(SurvivalStatType type, int value)
        {
            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                stat.Value = value;
            }
        }

        public void Update()
        {
            foreach (var stat in Stats)

            {
                ChangeStatInner(stat, -stat.Rate);
            }
        }

        public static SurvivalData CreateHumanPersonSurvival(IPersonScheme personScheme)
        {
            var stats = new[] {
                new SurvivalStat(personScheme.Hp, 0, personScheme.Hp){
                    Type = SurvivalStatType.Health
                },
                CreateStat(SurvivalStatType.Satiety),
                CreateStat(SurvivalStatType.Water)
            };

            return new SurvivalData(stats);
        }

        public static SurvivalData CreateMonsterPersonSurvival(IMonsterScheme monsterScheme)
        {
            var stats = new[] {
               new SurvivalStat(monsterScheme.Hp, 0, monsterScheme.Hp){
                    Type = SurvivalStatType.Health
                }
            };

            return new SurvivalData(stats);
        }

        private void ChangeStatInner(SurvivalStat stat, int value)
        {
            var oldValue = stat.Value;

            stat.Value += value;

            if (stat.KeyPoints != null)
            {
                CheckStatKeyPoints(stat, oldValue);
            }

            CheckHp(stat);
        }

        private void CheckStatKeyPoints(SurvivalStat stat, int oldValue)
        {
            var diff = RangeHelper.CreateNormalized(oldValue, stat.Value);

            var orientedKeyPoints = stat.KeyPoints;
            if (!diff.IsAcs)
            {
                orientedKeyPoints = stat.KeyPoints.Reverse().ToArray();
            }

            var crossedKeyPoints = new List<SurvivalStatKeyPoint>();
            foreach (var keyPoint in orientedKeyPoints)
            {
                if (diff.Contains(keyPoint.Value))
                {
                    crossedKeyPoints.Add(keyPoint);
                }
            }

            if (crossedKeyPoints.Any())
            {
                DoStatCrossKeyPoint(stat, crossedKeyPoints);
            }
        }

        private void CheckHp(SurvivalStat stat)
        {
            if (stat.Type == SurvivalStatType.Health)
            {
                var hp = stat.Value;
                if (hp <= 0)
                {
                    IsDead = true;
                    DoDead();
                }
            }
        }

        private void DoDead()
        {
            Dead?.Invoke(this, new EventArgs());
        }

        private static SurvivalStat CreateStat(SurvivalStatType type)
        {
            var stat = new SurvivalStat(50, -100, 100)
            {
                Type = type,
                Rate = 1,
                KeyPoints = new[]{
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, -50),
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, -25),
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 0)
                    }
            };
            return stat;
        }

        private void DoStatCrossKeyPoint(SurvivalStat stat, IEnumerable<SurvivalStatKeyPoint> keyPoints)
        {
            var args = new SurvivalStatChangedEventArgs(stat, keyPoints);
            StatCrossKeyValue?.Invoke(this, args);
        }
    }
}
