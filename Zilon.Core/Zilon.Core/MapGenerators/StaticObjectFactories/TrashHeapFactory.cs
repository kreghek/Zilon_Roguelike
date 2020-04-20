using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class TrashHeapFactory : PropDepositFactoryBase
    {
        public TrashHeapFactory(
            PropContainerPurpose propContainerPurpose,
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] { "shovel" }, dropTableSchemeSid: "trash-heap", propContainerPurpose, schemeService, dropResolver)
        {
        }
    }
}
