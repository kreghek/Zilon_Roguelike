using System;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Данные о выживании для монстра.
    /// </summary>
    public sealed class MonsterSurvivalData : ISurvivalData
    {
        public MonsterSurvivalData(IMonsterScheme monsterScheme)
        {
            Stats = new[] {
                new SurvivalStat(monsterScheme.Hp, 0, monsterScheme.Hp){
                    Type = SurvivalStatType.Health
                }
            };
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

            foreach (var keyPoint in stat.KeyPoints)
            {
                if (diff.Contains(keyPoint.Value))
                {
                    DoStatCrossKeyPoint(stat, keyPoint);
                }
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

        private void DoStatCrossKeyPoint(SurvivalStat stat, SurvivalStatKeyPoint keyPoint)
        {
            var args = new SurvivalStatChangedEventArgs(stat, keyPoint);
            StatCrossKeyValue?.Invoke(this, args);
        }
    }
}
