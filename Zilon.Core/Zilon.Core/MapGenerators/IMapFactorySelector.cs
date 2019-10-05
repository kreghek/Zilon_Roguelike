using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public interface IMapFactorySelector
    {
        IMapFactory GetMapFactory(ISectorSubScheme sectorScheme);        
    }
}
