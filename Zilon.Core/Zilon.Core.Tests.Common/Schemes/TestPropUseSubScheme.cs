using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropUseSubScheme : IPropUseSubScheme
    {
        public bool Consumable { get; set; }

        public ConsumeCommonRule[] CommonRules { get; set; }
    }
}