using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class StoneDepositFactory : PropDepositFactoryBase
    {
        public StoneDepositFactory(
            PropContainerPurpose propContainerPurpose,
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] { "pick-axe" }, dropTableSchemeSid: "stone-deposit", propContainerPurpose, schemeService, dropResolver)
        {
        }
    }
}
