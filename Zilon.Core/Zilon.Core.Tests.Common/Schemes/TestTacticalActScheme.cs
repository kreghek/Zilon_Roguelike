using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestTacticalActScheme : SchemeBase, ITacticalActScheme
    {
        public ITacticalActConstrainsSubScheme Constrains { get; set; }
        public ITacticalActStatsSubScheme Stats { get; set; }
        public string IsMimicFor { get; }
    }
}