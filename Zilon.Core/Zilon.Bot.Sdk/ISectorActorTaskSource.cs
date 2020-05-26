using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Sdk
{
    public interface ISectorActorTaskSource: IActorTaskSource<ISectorTaskSourceContext>
    {
        void Configure(IBotSettings botSettings);
    }
}
