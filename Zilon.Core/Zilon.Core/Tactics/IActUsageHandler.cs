using System;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Обработчик применения действия по типу цели.
    /// </summary>
    public interface IActUsageHandler
    {
        /// <summary>
        /// Тип цели, которую может принять разработчик.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Обработать применение действия к цели.
        /// </summary>
        /// <param name="actor"> Актёр, совершивший действие. </param>
        /// <param name="target"> Цель применения действия. </param>
        /// <param name="tacticalActRoll"> Соверщенное действие и его эффективность. </param>
        void ProcessActUsage(IActor actor, IAttackTarget target, TacticalActRoll tacticalActRoll);
    }
}
