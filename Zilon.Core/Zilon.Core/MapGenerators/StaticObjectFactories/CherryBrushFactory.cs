﻿using System;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class CherryBrushFactory : PropDepositFactoryBase
    {
        public CherryBrushFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(Array.Empty<string>(), "cherry-brush", PropContainerPurpose.CherryBrush,
            schemeService, dropResolver)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;

        protected override int ExhausingValue => 1;
    }
}