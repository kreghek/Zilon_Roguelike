using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class TrashHeapFactory : PropDepositFactoryBase
    {
        public TrashHeapFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] {"shovel"}, dropTableSchemeSid: "trash-heap",
            PropContainerPurpose.TrashHeap, schemeService, dropResolver)
        {
        }

        protected override int ExhausingValue => 3;
        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;
    }
}