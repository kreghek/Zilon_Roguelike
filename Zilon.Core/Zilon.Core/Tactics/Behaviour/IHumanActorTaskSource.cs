using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Interface to control person using UI.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IHumanActorTaskSource<TContext> : IActorTaskSource<TContext>
        where TContext : ISectorTaskSourceContext
    {
        /// <summary>
        /// Can user send a intetions?
        /// </summary>
        /// <returns><c>true</c> if the task source are ready to recieve new intention. <c>false</c> - otherwise.</returns>
        bool CanIntent();

        /// <summary>
        /// Drop waitong for next intention.
        /// Used then player person die or game exit.
        /// </summary>
        void DropIntentionWaiting();

        /// <summary>
        /// Send next intention of the user to the player person act.
        /// </summary>
        /// <param name="intention"> Intention object which will create actor task. </param>
        /// <param name="activeActor"> The player actor task which will act. </param>
        [System.Obsolete("Use async version instead.")]
        void Intent(IIntention intention, IActor activeActor);

        /// <summary>
        /// Send next intention of the user to the player person act.
        /// </summary>
        /// <param name="intention"> Intention object which will create actor task. </param>
        /// <param name="activeActor"> The player actor task which will act. </param>
        Task IntentAsync(IIntention intention, IActor activeActor);
    }
}