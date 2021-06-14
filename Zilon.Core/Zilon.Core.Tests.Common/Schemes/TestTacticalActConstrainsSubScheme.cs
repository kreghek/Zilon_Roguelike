using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestTacticalActConstrainsSubScheme : ITacticalActConstrainsSubScheme
    {
        public string PropResourceType { get; set; }
        public int? PropResourceCount { get; set; }
        public int? Cooldown { get; }
    }
}