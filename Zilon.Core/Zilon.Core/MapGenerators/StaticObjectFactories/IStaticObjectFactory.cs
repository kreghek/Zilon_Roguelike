using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public interface IStaticObjectFactory
    {
        PropContainerPurpose Purpose { get; }

        IStaticObject Create(ISector sector, HexNode node, int id);
    }
}