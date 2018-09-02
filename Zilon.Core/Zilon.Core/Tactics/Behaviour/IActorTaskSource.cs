using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Источник задач для актёров (управление пользователя или AI).
    /// </summary>
    public interface IActorTaskSource
    {
        /// <summary>
        /// Возвращает набор задач для указанного актёра.
        /// </summary>
        /// <param name="actor"> Актёр, для которого определяются задачи. </param>
        /// <returns> Набор задач актёра или пустой набор. </returns>
        /// <remarks>
        /// Всего будет 3 источника задач:
        /// 1. Источник задач для активного актёра от игрока.
        /// 2. Источник задач для монстров.
        /// 3. Источник задач для актёров игрока, которые не находятся
        /// под прямым управлением игрока (напарники по группе, неактивные ключевые актёры).
        /// </remarks>
        Task<IActorTask[]> GetActorTasks(IActor actor);
    }
}