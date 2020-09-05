using Zenject;

using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.Globe
{
    public class OnlyBotTaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource<ISectorTaskSourceContext> _botActorTaskSource;

        public OnlyBotTaskSourceCollector([Inject(Id = "monster")] IActorTaskSource<ISectorTaskSourceContext> botActorTaskSource)
        {
            _botActorTaskSource = botActorTaskSource;
        }

        public IActorTaskSource<ISectorTaskSourceContext>[] GetCurrentTaskSources()
        {
            return new[] { _botActorTaskSource };
        }
    }
}
