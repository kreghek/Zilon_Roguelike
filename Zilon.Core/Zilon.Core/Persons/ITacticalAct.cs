using Zilon.Core.Common;
using Zilon.Core.Props;
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
        ITacticalActScheme Scheme { get; }

        /// <summary>
        /// Схема основных характеристик тактического действия.
        /// </summary>
        ITacticalActStatsSubScheme Stats { get; }

        /// <summary>
        /// Подсхема с ограничениями на использование действий.
        /// </summary>
        ITacticalActConstrainsSubScheme Constrains { get; }

        /// <summary>
        /// Предмет экипировки, который даёт данное действие.
        /// </summary>
        Equipment Equipment { get; }

        /// <summary>
        /// Актуальные данные об эффективности действия.
        /// </summary>
        Roll Efficient { get; }

        /// <summary>
        /// Актуальные данные о применении действия.
        /// </summary>
        Roll ToHit { get; }

        /// <summary>
        /// Доступность действия.
        /// Доступность действия проверяется каждый ход. Действия доступны,
        /// если был бросок выше определённого значения (4+).
        /// </summary>
        bool IsAvailable { get; set; }
    }
}
