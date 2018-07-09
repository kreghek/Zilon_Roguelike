using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Источник задач для актёров (управление пользователя или AI).
    /// </summary>
    public interface IActorTaskSource
    {
        /// <summary>
        /// Возвращает набор задач для актёров.
        /// </summary>
        /// <param name="map"> Текущая карта. </param>
        /// <param name="actorManager"> Менеджер актёров. </param>
        /// <returns></returns>
        IActorTask[] GetActorTasks(IMap map, IActorManager actorManager);
    }
}