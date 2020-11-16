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
        /// Подсхема с ограничениями на использование действий.
        /// </summary>
        ITacticalActConstrainsSubScheme Constrains { get; }

        /// <summary>
        /// Текущее состояние КД на использование.
        /// Используется, если в схеме <see cref="ITacticalActConstrainsSubScheme.Cooldown"/> не null.
        /// </summary>
        int? CurrentCooldown { get; }

        /// <summary>
        /// Актуальные данные об эффективности действия.
        /// </summary>
        Roll Efficient { get; }

        /// <summary>
        /// Предмет экипировки, который даёт данное действие.
        /// </summary>
        Equipment Equipment { get; }

        /// <summary>
        /// Схема действия.
        /// </summary>
        ITacticalActScheme Scheme { get; }

        /// <summary>
        /// Схема основных характеристик тактического действия.
        /// </summary>
        ITacticalActStatsSubScheme Stats { get; }

        /// <summary>
        /// Актуальные данные о применении действия.
        /// </summary>
        Roll ToHit { get; }

        /// <summary>
        /// Сброс счётчика КД.
        /// Применяется, когда действие выполнено. Начинает новый отсчёт.
        /// </summary>
        void StartCooldownIfItIs();

        /// <summary>
        /// Обновление состояния КД.
        /// Уменьшает отсчёт, чтобы, со временем, действие снова стало доступным.
        /// </summary>
        void UpdateCooldown();
    }
}