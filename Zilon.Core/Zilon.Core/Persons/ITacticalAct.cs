using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{

    /// <summary>
    /// Интерфейс тактического действия.
    /// </summary>
    public interface ITacticalAct
    {
        /// <summary>
        /// Схема действия.
        /// </summary>
        TacticalActScheme Scheme { get; }

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
