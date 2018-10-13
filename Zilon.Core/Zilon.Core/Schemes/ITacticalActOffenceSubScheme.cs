using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс для подсхемы для атакующих действий.
    /// </summary>
    public interface ITacticalActOffenceSubScheme
    {
        /// <summary>
        /// Тип действия.
        /// </summary>
        OffenseType Type { get; }

        /// <summary>
        /// Тип воздействия.
        /// </summary>
        ImpactType Impact { get; }

        /// <summary>
        /// Ранг пробития брони.
        /// </summary>
        /// <remarks>
        /// Если ранк пробития больше, чем ранк брони цели,
        /// то цень не пробрасывает спас-бросок за броню.
        /// </remarks>
        int ApRank { get; }
    }
}