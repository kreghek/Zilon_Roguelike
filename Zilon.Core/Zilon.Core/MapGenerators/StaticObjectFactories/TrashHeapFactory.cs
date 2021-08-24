﻿using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [ExcludeFromCodeCoverage]
    public sealed class TrashHeapFactory : PropDepositFactoryBase
    {
        public TrashHeapFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: new[] { "shovel" }, dropTableSchemeSid: "trash-heap",
            PropContainerPurpose.TrashHeap, schemeService, dropResolver, false)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;

        protected override int ExhausingValue => 3;
    }
}