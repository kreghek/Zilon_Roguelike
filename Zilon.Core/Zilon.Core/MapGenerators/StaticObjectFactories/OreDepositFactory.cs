using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [ExcludeFromCodeCoverage]
    public sealed class OreDepositFactory : PropDepositFactoryBase
    {
        public OreDepositFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] { "pick-axe" }, dropTableSchemeSid: "ore-deposit",
            PropContainerPurpose.OreDeposits, schemeService, dropResolver, true)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Moderately;

        protected override int ExhausingValue => 10;
    }
}