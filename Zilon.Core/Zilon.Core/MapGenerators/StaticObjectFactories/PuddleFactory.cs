using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [ExcludeFromCodeCoverage]
    public sealed class PuddleFactory : PropDepositFactoryBase
    {
        public PuddleFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(Array.Empty<string>(), "water-puddle", PropContainerPurpose.Puddle,
            schemeService, dropResolver, false)
        {
        }

        protected override DepositMiningDifficulty Difficulty => DepositMiningDifficulty.Easy;

        protected override int ExhausingValue => 1;
    }
}