using JetBrains.Annotations;

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
        /// <param name="actor"> Активный актёр игрока. </param>
        /// <returns> Возвращает задачу для указанного актёра. </returns>
        IActorTask CreateActorTask([NotNull] IActor actor);
    }
}