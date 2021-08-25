﻿using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [ExcludeFromCodeCoverage]
    public sealed class StoneDepositFactory : PropDepositFactoryBase
    {
        public StoneDepositFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] { "pick-axe" }, dropTableSchemeSid: "stone-deposit",
            PropContainerPurpose.StoneDeposits, schemeService, dropResolver, true)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Moderately;

        protected override int ExhausingValue => 10;
    }
}