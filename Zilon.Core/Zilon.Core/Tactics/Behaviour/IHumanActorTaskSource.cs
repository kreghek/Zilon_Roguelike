namespace Zilon.Core.Tactics.Behaviour
{
    public interface IHumanActorTaskSource: IActorTaskSource
    {
        /// <summary>
        /// Переключает текущего ключевого актёра.
        /// </summary>
        /// <param name="currentActor"> Целевой клчевой актёр. </param>
        void SwitchActor(IActor currentActor);

        /// <summary>
        /// Текущий активный ключевой актёр.
        /// </summary>
        IActor CurrentActor { get; }

        void Intent(IIntention intension);
    }
}