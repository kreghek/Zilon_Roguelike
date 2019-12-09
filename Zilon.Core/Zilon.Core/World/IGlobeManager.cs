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
        Globe Globe { get; set; }

        /// <summary>
        /// Обновление состояния мира на один шаг.
        /// </summary>
        /// <param name="botTaskSource">Источник команд для ботов.</param>
        Task UpdateGlobeOneStep(IActorTaskSource botTaskSource);
    }
}
