using Zenject;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Assets.Zilon.Scripts.Services
{
    internal class MapFactorySelector : IMapFactorySelector
    {
        private readonly IMapFactory _caveMapFactory;
        private readonly IMapFactory _roomMapFactory;

        public MapFactorySelector([Inject(Id = "cave")] IMapFactory caveMapFactory, [Inject(Id = "room")] IMapFactory roomMapFactory)
        {
            _caveMapFactory = caveMapFactory;
            _roomMapFactory = roomMapFactory;
        }

        public IMapFactory GetMapFactory(ISectorSubScheme sectorScheme)
        {
            switch (sectorScheme.Sid)
            {
                case "genomass-cave":
                    return _caveMapFactory;

                default:
                    return _roomMapFactory;
            }
        }
    }
}
