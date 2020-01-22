using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    /// <summary>
    /// Сервис, управляющий глобальной картой.
    /// </summary>
    public interface IGlobeManager
    {
        /// <summary>
        /// Глобальная карта.
        /// </summary>
        Globe Globe { get; }

        /// <summary>
        /// Указывает, выполнена ли инициализация мира.
        /// </summary>
        bool IsGlobeInitialized { get; }

        /// <summary>
        /// Инициализация текущего мира.
        /// </summary>
        Task InitializeGlobeAsync();

        /// <summary>
        /// Обновление состояния мира на один шаг.
        /// </summary>
        /// <param name="context">Контекст обновления.</param>
        Task UpdateGlobeOneStepAsync(GlobeIterationContext context);

        /// <summary>
        /// Сбрасывает текущее состояние мира. Переходит в состояние неинициализированного.
        /// </summary>
        void ResetGlobeState();
    }
}
