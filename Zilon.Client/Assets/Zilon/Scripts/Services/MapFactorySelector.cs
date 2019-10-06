using Zenject;

using Zilon.Core.MapGenerators;

namespace Assets.Zilon.Scripts.Services
{
    internal class MapFactorySelector : SwitchMapFactorySelectorBase
    {
        private readonly IMapFactory _cellularAutomatonMapFactory;
        private readonly IMapFactory _roomMapFactory;

        public MapFactorySelector(
            [Inject(Id = "cave")] IMapFactory cellularAutomatonMapFactory,
            [Inject(Id = "room")] IMapFactory roomMapFactory)
        {
            _cellularAutomatonMapFactory = cellularAutomatonMapFactory;
            _roomMapFactory = roomMapFactory;
        }

        protected override IMapFactory CellularAutomatonMapFactory => _cellularAutomatonMapFactory;
        protected override IMapFactory RoomMapFactory => _roomMapFactory;
    }
}
