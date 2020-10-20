using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Sdk
{
    public interface ISectorActorTaskSource<TContext>: IActorTaskSource<TContext> where TContext: ISectorTaskSourceContext
    {
        void Configure(IBotSettings botSettings);
    }
}