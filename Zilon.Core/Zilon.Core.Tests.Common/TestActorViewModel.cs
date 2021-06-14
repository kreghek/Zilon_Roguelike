using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Common
{
    [ExcludeFromCodeCoverage]
    public sealed class TestActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}