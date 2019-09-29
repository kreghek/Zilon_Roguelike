using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestPersonSurvivalStatKeyPointSubScheme: IPersonSurvivalStatKeyPointSubScheme
    {
        public PersonSurvivalStatKeypointLevel Level { get; set; }

        public int Value { get; set; }
    }
}
