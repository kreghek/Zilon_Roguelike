namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Интерфейс для указания намерений игрока.
    /// </summary>
    public interface IIntention
    {
        /// <summary>
        /// Создание задачи актёра на основе намерения.
        /// </summary>
        /// <param name="currentTask"> Текущая задача в источнике команд. </param>
        /// <param name="actor"> Активный актёр игрока. </param>
        /// <returns> Возвращает задачу для указанного актёра. </returns>
        IActorTask CreateActorTask(IActorTask currentTask, IActor actor);
    }
}
