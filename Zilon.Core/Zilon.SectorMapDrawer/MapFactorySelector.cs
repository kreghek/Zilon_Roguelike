using Zilon.Core.MapGenerators;

namespace Zilon.SectorGegerator
{
    class MapFactorySelector : SwitchMapFactorySelectorBase
    {
        private readonly IMapFactory _cellularAutomatorMapFactory;
        private readonly IMapFactory _roomMapfactory;

        public MapFactorySelector(IMapFactory cellularAutomatorMapFactory, IMapFactory roomMapfactory)
        {
            _cellularAutomatorMapFactory = cellularAutomatorMapFactory;
            _roomMapfactory = roomMapfactory;
        }

        protected override IMapFactory CellularAutomatonMapFactory { get => _cellularAutomatorMapFactory; }
        protected override IMapFactory RoomMapFactory { get => _roomMapfactory; }
    }
}
