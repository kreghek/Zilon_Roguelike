using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class TacticalActUsageService : ITacticalActUsageService
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
            var rolledEfficient = _actUsageRandomSource.RollEfficient(act.Efficient);

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
            var defenceType = GetDefence(offenceType);
            var currentDefences = targetActor.Person.CombatStats.DefenceStats.Defences
                .Where(x => x.Type == defenceType || x.Type == DefenceType.DivineDefence);

            var prefferedDefenceItem = CalcPrefferedDefence(currentDefences);

            if (prefferedDefenceItem != null)
            {
                var successToHitRoll = CalcSuccessRoll(prefferedDefenceItem.Level);
                var factToHitRoll = _actUsageRandomSource.RollToHit();

                if (factToHitRoll >= successToHitRoll)
                {
                    targetActor.TakeDamage(tacticalActRoll.Efficient);

                    if (!targetIsDeadLast && targetActor.State.IsDead)
                    {
                        CountTargetActorDefeat(actor, targetActor);
                    }
                }
            }
            else
            {
                targetActor.TakeDamage(tacticalActRoll.Efficient);

                if (!targetIsDeadLast && targetActor.State.IsDead)
                {
                    CountTargetActorDefeat(actor, targetActor);
                }
            }
        }

        /// <summary>
        /// Рассчитывает минимальное значение броска D6, необходимого для пробития указанной обороны.
        /// </summary>
        /// <param name="level"> Уровень обороны, для которой вычисляется нижный порог броска D6. </param>
        /// <returns> Минимальный погод броска D6. </returns>
        private static int CalcSuccessRoll(PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.None:
                    return 2;

                case PersonRuleLevel.Lesser:
                    return 4;

                case PersonRuleLevel.Normal:
                    return 5;

                case PersonRuleLevel.Grand:
                    return 6;

                case PersonRuleLevel.Absolute:
                    return 8;

                default:
                    throw new ArgumentException($"Неизвестное значение {level}.", nameof(level));
            }
        }

        /// <summary>
        /// Возвращает оборону с наиболее предпочтительными характеристиками. Фактически, самого высокого уровня.
        /// </summary>
        /// <param name="currentDefences"> Текущие обороны. </param>
        /// <returns> Возвращает объект предпочтительной обороны. </returns>
        private PersonDefenceItem CalcPrefferedDefence(IEnumerable<PersonDefenceItem> currentDefences)
        {
            if (!currentDefences.Any())
            {
                return null;
            }

            var sortedDefences = currentDefences.OrderByDescending(x => x.Level);
            var prefferedDefence = sortedDefences.First();
            return prefferedDefence;
        }

        /// <summary>
        /// Возвращает тип обороны, которая может быть использована для отражения указанного наступления.
        /// </summary>
        /// <param name="offenceType"> Тип наступления. </param>
        /// <returns> Возвращает экземпляр типа обороны. </returns>
        private static DefenceType GetDefence(OffenseType offenceType)
        {
            var rawValue = (int)offenceType;
            var defenceType = (DefenceType)rawValue;
            return defenceType;
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
