using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public interface IStaticObjectFactory
    {
        PropContainerPurpose Purpose { get; }

        IStaticObject Create(ISector sector, Tactics.Spatial.HexNode node, int id);
    }
}