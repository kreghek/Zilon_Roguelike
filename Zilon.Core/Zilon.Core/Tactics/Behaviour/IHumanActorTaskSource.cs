namespace Zilon.Core.Tactics.Behaviour
{
    public interface IHumanActorTaskSource<TContext> : IActorTaskSource<TContext>
        where TContext : ISectorTaskSourceContext
    {
        bool CanIntent();

        void Intent(IIntention intention, IActor activeActor);

        Task IntentAsync(IIntention intention, IActor activeActor);
    }
}