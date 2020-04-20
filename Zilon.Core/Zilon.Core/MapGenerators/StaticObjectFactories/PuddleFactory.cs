using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class PuddleFactory : PropDepositFactoryBase
    {
        public PuddleFactory(
            ISchemeService schemeService,
            IDropResolver dropResolver) : base(toolTags: Array.Empty<string>(), dropTableSchemeSid: "water-puddle", PropContainerPurpose.Puddle, schemeService, dropResolver)
        {
        }
    }
}
