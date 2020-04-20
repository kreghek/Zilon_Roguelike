using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public sealed class StaticObstaclesGenerator : IStaticObstaclesGenerator
    {
        private readonly IChestGenerator _chestGenerator;
        private readonly IInteriorObjectRandomSource _interiorObjectRandomSource;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;

        public StaticObstaclesGenerator(IChestGenerator chestGenerator,
            IInteriorObjectRandomSource interiorObjectRandomSource,
            IDropResolver dropResolver,
            ISchemeService schemeService)
        {
            _chestGenerator = chestGenerator ?? throw new ArgumentNullException(nameof(chestGenerator));
            _interiorObjectRandomSource = interiorObjectRandomSource ?? throw new ArgumentNullException(nameof(interiorObjectRandomSource));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
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

                    // Все сгенерированные препятсивия - это руда.
                    // В последствии будет переделано.
                    // Должны генерироваться разные объекты - ямы, лужи, кусты, деревья.

                    // Все залежи изначально имеют пустой модуль контейнера.
                    // Он будет заполняться по мере добычи.
                    var containerModule = new DepositContainer();
                    staticObject.AddModule(containerModule);

                    var lifetimeModule = new LifetimeModule(sector.StaticObjectManager, staticObject);

                    var dropScheme = _schemeService.GetScheme<IDropTableScheme>("ore-deposit");
                    var toolScheme = _schemeService.GetScheme<IPropScheme>("pick-axe");
                    var depositModule = new PropDepositModule(containerModule, dropScheme, _dropResolver, toolScheme, lifetimeModule);
                    staticObject.AddModule(depositModule);

                    sector.StaticObjectManager.Add(staticObject);
                }
            }

            _chestGenerator.CreateChests(sector, sectorSubScheme, sector.Map.Regions);

            return Task.CompletedTask;
        }
    }
}
