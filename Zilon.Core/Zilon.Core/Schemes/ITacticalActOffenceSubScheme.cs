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
        TacticalActImpactType Impact { get; }
    }
}