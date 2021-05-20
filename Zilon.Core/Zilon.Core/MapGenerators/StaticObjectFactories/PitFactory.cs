﻿using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class PitFactory : IStaticObjectFactory
    {
        public PropContainerPurpose Purpose => PropContainerPurpose.Pit;

        public IStaticObject Create(ISector sector, HexNode node, int id)
        {
            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            var staticObject = new StaticObject(node, Purpose, id);

            return staticObject;
        }
    }
}