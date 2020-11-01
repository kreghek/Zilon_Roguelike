using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class CherryBrushFactory : PropDepositFactoryBase
    {
        public CherryBrushFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(Array.Empty<string>(), "cherry-brush", PropContainerPurpose.CherryBrush,
            schemeService, dropResolver)
        {
        }

        protected override int ExhausingValue => 1;
        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;
    }
}