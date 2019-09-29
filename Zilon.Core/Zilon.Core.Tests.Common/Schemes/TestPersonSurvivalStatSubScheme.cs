using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestPersonSurvivalStatSubScheme : IPersonSurvivalStatSubScheme
    {
        public PersonSurvivalStatType Type { get; set; }
        public IPersonSurvivalStatKeyPointSubScheme[] KeyPoints { get; set; }
        public int StartValue { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}
