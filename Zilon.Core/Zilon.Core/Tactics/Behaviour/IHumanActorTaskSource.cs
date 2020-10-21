using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IHumanActorTaskSource<TContext> : IActorTaskSource<TContext> where TContext : ISectorTaskSourceContext
    {
        Task IntentAsync(IIntention intention, IActor activeActor);

        void Intent(IIntention intention, IActor activeActor);

        bool CanIntent();
    }
}