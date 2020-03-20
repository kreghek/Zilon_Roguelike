using Zenject;

using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.Globe
{
    public class OnlyBotTaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource _botActorTaskSource;

        public OnlyBotTaskSourceCollector([Inject(Id = "monster")] IActorTaskSource botActorTaskSource)
        {
            _botActorTaskSource = botActorTaskSource;
        }

        public IActorTaskSource[] GetCurrentTaskSources()
        {
            return new[] { _botActorTaskSource };
        }
    }
}
