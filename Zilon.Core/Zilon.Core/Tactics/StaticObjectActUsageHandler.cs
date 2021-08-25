using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Обработчик действий, нацеленных на статический объект.
    /// </summary>
    public sealed class StaticObjectActUsageHandler : IActUsageHandler
    {
        /// <summary>
        /// Применяет действие на предмет, на который можно подействовать (сундук/дверь/камень).
        /// </summary>
        /// <param name="target"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private static void UseOnStaticObject(IAttackTarget target, CombatActRoll tacticalActRoll)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.TakeDamage(tacticalActRoll.Efficient);
        }

        /// <inheritdoc />
        public Type TargetType => typeof(IStaticObject);

        /// <inheritdoc />
        public void ProcessActUsage(IActor actor, IAttackTarget target, CombatActRoll tacticalActRoll, ISectorMap map)
        {
            if (tacticalActRoll is null)
            {
                throw new ArgumentNullException(nameof(tacticalActRoll));
            }

            UseOnStaticObject((IStaticObject)target, tacticalActRoll);
        }
    }
}