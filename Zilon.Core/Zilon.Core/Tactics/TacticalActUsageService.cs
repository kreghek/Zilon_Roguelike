using System;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class TacticalActUsageService : ITacticalActUsageService
    {
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;
        private readonly IPerkResolver _perkResolver;

        public TacticalActUsageService(ITacticalActUsageRandomSource actUsageRandomSource, IPerkResolver perkResolver)
        {
            _actUsageRandomSource = actUsageRandomSource;
            _perkResolver = perkResolver;
        }

        public void UseOn(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            //TODO реализовать возможность действовать на себя некоторыми скиллами.
            if (actor == target)
            {
                throw new ArgumentException("Актёр не может атаковать сам себя", nameof(target));
            }

            var currentCubePos = ((HexNode)actor.Node).CubeCoords;
            var targetCubePos = ((HexNode)target.Node).CubeCoords;

            var isInDistance = act.CheckDistance(currentCubePos, targetCubePos);
            if (!isInDistance)
            {
                throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }

            var tacticalActRoll = GetActEfficient(act);

            if (target is IActor targetActor)
            {
                UseOnActor(actor, targetActor, tacticalActRoll);
            }
            else
            {
                UseOnChest(target, tacticalActRoll);
            }
        }

        /// <summary>
        /// Возвращает случайное значение эффективность действия.
        /// </summary>
        /// <param name="act"> Соверщённое действие. </param>
        /// <returns> Возвращает выпавшее значение эффективности. </returns>
        private TacticalActRoll GetActEfficient(ITacticalAct act)
        {
            var rolledEfficient = _actUsageRandomSource.RollEfficient(act.Stats.Efficient);

            var roll = new TacticalActRoll(act, rolledEfficient);

            return roll;
        }

        /// <summary>
        /// Применяет действие на предмет, на который можно подействовать (сундук/дверь).
        /// </summary>
        /// <param name="target"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private static void UseOnChest(IAttackTarget target, TacticalActRoll tacticalActRoll)
        {
            target.TakeDamage(tacticalActRoll.Efficient);
        }

        /// <summary>
        /// Применяет действие на актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private void UseOnActor(IActor actor, IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            var targetIsDeadLast = targetActor.State.IsDead;

            var offenceType = tacticalActRoll.TacticalAct.Stats.Offence.Type;
            var defenceType = HitHelper.GetDefence(offenceType);
            var currentDefences = targetActor.Person.CombatStats.DefenceStats.Defences
                .Where(x => x.Type == defenceType || x.Type == DefenceType.DivineDefence);

            var prefferedDefenceItem = HitHelper.CalcPrefferedDefence(currentDefences);
            var successToHitRoll = HitHelper.CalcSuccessToHit(prefferedDefenceItem);
            var factToHitRoll = _actUsageRandomSource.RollToHit();

            if (factToHitRoll >= successToHitRoll)
            {
                var actApRank = GetActApRank(tacticalActRoll.TacticalAct);
                var armorRank = GetArmorRank(targetActor, tacticalActRoll.TacticalAct);

                var actEfficientArmorBlocked = tacticalActRoll.Efficient;

                if (actApRank <= armorRank)
                {
                    var factArmorSaveRoll = RollArmorSave(targetActor);
                    var successArmorSaveRoll = GetSuccessArmorSave(targetActor, tacticalActRoll.TacticalAct);
                    if (factArmorSaveRoll >= successArmorSaveRoll)
                    {
                        var armorAbsorbtion = GetArmorAbsorbtion(targetActor, tacticalActRoll.TacticalAct);
                        actEfficientArmorBlocked -= armorAbsorbtion;
                    }
                }

                targetActor.TakeDamage(actEfficientArmorBlocked);

                if (!targetIsDeadLast && targetActor.State.IsDead)
                {
                    CountTargetActorDefeat(actor, targetActor);
                }
            }
        }

        /// <summary>
        /// Возвращает показатель поглощения брони цели.
        /// Это величина, на которуб будет снижен урон.
        /// </summary>
        /// <param name="targetActor"></param>
        /// <returns></returns>
        private static int GetArmorAbsorbtion(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.CombatStats.DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offence.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            if (preferredArmor == null)
            {
                return 0;
            }

            switch (preferredArmor.AbsorbtionLevel)
            {
                case PersonRuleLevel.None:
                    return 0;

                case PersonRuleLevel.Lesser:
                    return 1;

                case PersonRuleLevel.Normal:
                    return 2;

                case PersonRuleLevel.Grand:
                    return 5;

                case PersonRuleLevel.Absolute:
                    return 10;

                default:
                    throw new InvalidOperationException($"Неизвестный уровень поглощения брони {preferredArmor.AbsorbtionLevel}.");
            }
        }

        /// <summary>
        /// Рассчитывает успешный спас-бросок за броню цели.
        /// </summary>
        /// <param name="targetActor"></param>
        /// <returns></returns>
        /// <remarks>
        /// При равных рангах броня пробивается на 4+.
        /// За каждые два ранга превосходства действия над бронёй - увеличение на 1.
        /// </remarks>
        private static int GetSuccessArmorSave(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.CombatStats.DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offence.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            if (preferredArmor == null)
            {
                return 4;
            }

            var apRankDiff = usedTacticalAct.Stats.Offence.ApRank - preferredArmor.ArmorRank;

            var successRoll = 6;
            if (apRankDiff <= 1)
            {
                return successRoll;
            }
            else
            {
                return successRoll - (apRankDiff - 1) / 2;
            }
        }

        /// <summary>
        /// Возвращает результат спас-броска на броню.
        /// </summary>
        /// <param name="targetActor"></param>
        /// <returns></returns>
        private int RollArmorSave(IActor targetActor)
        {
            var factRoll = _actUsageRandomSource.RollArmorSave();
            return factRoll;
        }

        /// <summary>
        /// Возвращает ранг брони цели.
        /// </summary>
        /// <param name="targetActor"></param>
        /// <returns></returns>
        private static int GetArmorRank(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.CombatStats.DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offence.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            if (preferredArmor == null)
            {
                return 0;
            }

            return preferredArmor.ArmorRank;
        }

        /// <summary>
        /// Возвращает ранг пробития действия.
        /// </summary>
        /// <param name="tacticalAct"></param>
        /// <returns></returns>
        private int GetActApRank(ITacticalAct tacticalAct)
        {
            return tacticalAct.Stats.Offence.ApRank;
        }

        /// <summary>
        /// Расчитывает убийство целевого актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        private void CountTargetActorDefeat(IActor actor, IActor targetActor)
        {
            if (actor.Person is MonsterPerson)
            {
                // Монстры не могут прокачиваться.
                return;
            }

            var evolutionData = actor.Person.EvolutionData;

            var defeatProgress = new DefeatActorJobProgress(targetActor);

            _perkResolver.ApplyProgress(defeatProgress, evolutionData);
        }
    }
}
