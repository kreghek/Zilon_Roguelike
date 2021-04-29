using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestPropUseSubScheme : IPropUseSubScheme
    {
        public bool Consumable { get; set; }
        public ConsumeCommonRule[] CommonRules { get; set; }
        public IUsageRestrictions Restrictions { get; set; }
    }
}