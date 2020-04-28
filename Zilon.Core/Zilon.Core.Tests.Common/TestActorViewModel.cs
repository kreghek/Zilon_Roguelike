using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Common
{
    public sealed class TestActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item { get => Actor; }
    }
}
