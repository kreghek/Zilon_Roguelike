using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public abstract class SwitchMapFactorySelectorBase : IMapFactorySelector
    {
        public IMapFactory GetMapFactory(ISectorSubScheme sectorScheme)
        {
            switch (sectorScheme.MapGenerator)
            {
                case "CellularAutomaton":
                    return CellularAutomatonMapFactory;

                default:
                    return RoomMapFactory;
            }
        }

        protected abstract IMapFactory CellularAutomatonMapFactory { get; }

        protected abstract IMapFactory RoomMapFactory { get; }
    }
}
