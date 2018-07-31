using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{

    /// <summary>
    /// Интерфейс тактического действия.
    /// </summary>
    public interface ITacticalAct
    {
        /// <summary>
        /// Схема основных характеристик тактического действия.
        /// </summary>
        TacticalActStatsSubScheme Stats { get; }

        /// <summary>
        /// Минимальная эффективность действия.
        /// </summary>
        float MinEfficient { get; }

        /// <summary>
        /// Максимальная эффективность действия.
        /// </summary>
        float MaxEfficient { get; }
    }
}
