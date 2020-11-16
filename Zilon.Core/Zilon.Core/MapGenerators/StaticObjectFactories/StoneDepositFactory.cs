using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class StoneDepositFactory : PropDepositFactoryBase
    {
        public StoneDepositFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[]
            {
                "pick-axe"
            }, dropTableSchemeSid: "stone-deposit",
            PropContainerPurpose.StoneDeposits, schemeService, dropResolver)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Moderately;

        protected override int ExhausingValue => 10;
    }
}