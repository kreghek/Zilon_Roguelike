using System;
using System.Collections.Generic;
using System.Text;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public interface IStaticObjectfactoryCollector
    {
        IStaticObjectFactory[] GetFactories();
    }
}
