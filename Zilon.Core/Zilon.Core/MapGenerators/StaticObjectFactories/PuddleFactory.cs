﻿using System;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class PuddleFactory : PropDepositFactoryBase
    {
        public PuddleFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(Array.Empty<string>(), "water-puddle", PropContainerPurpose.Puddle,
            schemeService, dropResolver)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;

        protected override int ExhausingValue => 1;
    }
}