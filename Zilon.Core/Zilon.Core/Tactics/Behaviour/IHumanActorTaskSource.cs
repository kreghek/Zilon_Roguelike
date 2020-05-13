namespace Zilon.Core.Tactics.Behaviour
{
    public interface IHumanActorTaskSource: IActorTaskSource
    {
        /// <summary>
        /// Переключает текущего ключевого актёра.
        /// </summary>
        /// <param name="currentActor"> Целевой клчевой актёр. </param>
        void SwitchActiveActor(IActor currentActor);

        /// <summary>
        /// Текущий активный ключевой актёр.
        /// </summary>
        IActor ActiveActor { get; }

        void Intent(IIntention intention);
    }
}