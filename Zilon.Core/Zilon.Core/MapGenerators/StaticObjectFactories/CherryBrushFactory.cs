using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [ExcludeFromCodeCoverage]
    public sealed class CherryBrushFactory : PropDepositFactoryBase
    {
        public CherryBrushFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(Array.Empty<string>(), "cherry-brush", PropContainerPurpose.CherryBrush,
            schemeService, dropResolver, true)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;

        protected override int ExhausingValue => 1;
    }
}