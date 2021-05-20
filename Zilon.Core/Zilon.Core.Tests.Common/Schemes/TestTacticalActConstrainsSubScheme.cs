using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestTacticalActConstrainsSubScheme : ITacticalActConstrainsSubScheme
    {
        public string PropResourceType { get; set; }
        public int? PropResourceCount { get; set; }
        public int? Cooldown { get; }
    }
}