using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Bot
{
    public sealed class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
    }
}
