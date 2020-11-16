using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class OreDepositFactory : PropDepositFactoryBase
    {
        public OreDepositFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] { "pick-axe" }, dropTableSchemeSid: "ore-deposit",
            PropContainerPurpose.OreDeposits, schemeService, dropResolver)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Moderately;

        protected override int ExhausingValue => 10;
    }
}