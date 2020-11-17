using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.TextClient
{
    class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item { get => Actor; }
    }
}