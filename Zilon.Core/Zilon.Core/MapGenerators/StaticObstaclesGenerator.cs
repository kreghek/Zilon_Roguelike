using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public sealed class StaticObstaclesGenerator : IStaticObstaclesGenerator
    {
        private readonly IChestGenerator _chestGenerator;
        private readonly IInteriorObjectRandomSource _interiorObjectRandomSource;

        public StaticObstaclesGenerator(IChestGenerator chestGenerator, IInteriorObjectRandomSource interiorObjectRandomSource)
        {
            _chestGenerator = chestGenerator;
            _interiorObjectRandomSource = interiorObjectRandomSource;
        }

        public Task CreateAsync(ISector sector, ISectorSubScheme sectorSubScheme)
        {
            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            // Генерация препятсвий, как статических объектов.
            foreach (var region in sector.Map.Regions)
            {
                var regionNodes = region.Nodes.Cast<HexNode>().ToArray();
                var regionCoords = regionNodes.Select(x => x.OffsetCoords).ToArray();
                var interiorMetas = _interiorObjectRandomSource.RollInteriorObjects(regionCoords);

                foreach (var interior in interiorMetas)
                {
                    var node = regionNodes.Single(x => x.OffsetCoords == interior.Coords);
                    var staticObject = new StaticObject(node, default);
                    sector.StaticObjectManager.Add(staticObject);
                }
            }

            _chestGenerator.CreateChests(sector, sectorSubScheme, sector.Map.Regions);

            return Task.CompletedTask;
        }
    }
}
