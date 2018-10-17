using Zilon.Core.Common;
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
        ITacticalActStatsSubScheme Stats { get; }

        /// <summary>
        /// Актуальные данные об эффективности действия.
        /// </summary>
        Roll Efficient { get; }
    }
}
