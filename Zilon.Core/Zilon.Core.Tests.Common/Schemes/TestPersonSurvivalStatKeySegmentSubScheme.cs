using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestPersonSurvivalStatKeySegmentSubScheme : IPersonSurvivalStatKeySegmentSubScheme
    {
        public PersonSurvivalStatKeypointLevel Level { get; set; }

        public float Start { get; set; }

        public float End { get; set; }
    }
}