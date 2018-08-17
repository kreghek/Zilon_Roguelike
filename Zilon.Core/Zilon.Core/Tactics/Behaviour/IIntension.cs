namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Интерфейс для указания намерений игрока.
    /// </summary>
    public interface IIntension
    {
        /// <summary>
        /// Создание задачи актёра на основе намерения.
        /// </summary>
        /// <param name="currentIntension"> Текущее намерение игрока. </param>
        /// <param name="actor"> Активный актёр игрока. </param>
        /// <returns> Возвращает задачу для указанного актёра. </returns>
        IActorTask CreateActorTask(IIntension currentIntension, IActor actor);
    }
}
