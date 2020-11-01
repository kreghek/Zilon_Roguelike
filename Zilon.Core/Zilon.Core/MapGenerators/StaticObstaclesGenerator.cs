using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.StaticObjectFactories;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    public sealed class StaticObstaclesGenerator : IStaticObstaclesGenerator
    {
        private readonly IChestGenerator _chestGenerator;
        private readonly IInteriorObjectRandomSource _interiorObjectRandomSource;
        private readonly IStaticObjectFactoryCollector _staticObjectfactoryCollector;
        private readonly IStaticObjectsGeneratorRandomSource _staticObjectsGeneratorRandomSource;

        public StaticObstaclesGenerator(IChestGenerator chestGenerator,
            IInteriorObjectRandomSource interiorObjectRandomSource,
            IStaticObjectFactoryCollector staticObjectfactoryCollector,
            IStaticObjectsGeneratorRandomSource staticObjectsGeneratorRandomSource)
        {
            _chestGenerator = chestGenerator ?? throw new ArgumentNullException(nameof(chestGenerator));
            _interiorObjectRandomSource = interiorObjectRandomSource ??
                                          throw new ArgumentNullException(nameof(interiorObjectRandomSource));
            _staticObjectfactoryCollector = staticObjectfactoryCollector ??
                                            throw new ArgumentNullException(nameof(staticObjectfactoryCollector));
            _staticObjectsGeneratorRandomSource = staticObjectsGeneratorRandomSource ??
                                                  throw new ArgumentNullException(
                                                      nameof(staticObjectsGeneratorRandomSource));
        }

        public Task CreateAsync(IStaticObjectGenerationContext generationContext)
        {
            if (generationContext is null)
            {
                throw new ArgumentNullException(nameof(generationContext));
            }

            ISector sector = generationContext.Sector;

            var exitNodes = sector.Map.Transitions.Keys.Cast<HexNode>().Select(x => x.OffsetCoords).ToArray();

            // Генерация препятсвий, как статических объектов.
            foreach (var region in sector.Map.Regions)
            {
                var regionNodes = region.Nodes.Cast<HexNode>().ToArray();
                var regionCoords = regionNodes.Select(x => x.OffsetCoords).Except(exitNodes).ToArray();
                InteriorObjectMeta[] interiorMetas = _interiorObjectRandomSource.RollInteriorObjects(regionCoords);

                foreach (InteriorObjectMeta interior in interiorMetas)
                {
                    var node = regionNodes.Single(x => x.OffsetCoords == interior.Coords);
                    IResourceDepositData resourceDepositData = generationContext.ResourceDepositData;
                    IStaticObject staticObject = CreateStaticObject(sector, node, resourceDepositData);

                    sector.StaticObjectManager.Add(staticObject);
                }
            }

            ISectorSubScheme sectorSubScheme = generationContext.Scheme;
            _chestGenerator.CreateChests(sector, sectorSubScheme, sector.Map.Regions);

            return Task.CompletedTask;
        }

        private IStaticObject CreateStaticObject(ISector sector, HexNode node, IResourceDepositData resourceDepositData)
        {
            PropContainerPurpose staticObjectPurpose = RollPurpose(resourceDepositData);

            IStaticObjectFactory factory = SelectStaticObjectFactory(staticObjectPurpose);

            IStaticObject staticObject = factory.Create(sector, node, default);

            return staticObject;
        }

        private IStaticObjectFactory SelectStaticObjectFactory(PropContainerPurpose staticObjectPurpose)
        {
            IStaticObjectFactory[] factories = _staticObjectfactoryCollector.GetFactories();

            foreach (IStaticObjectFactory factory in factories)
            {
                if (factory.Purpose != staticObjectPurpose)
                {
                    continue;
                }

                return factory;
            }

            throw new InvalidOperationException(
                $"Не обнаружена фабрика для статических объектов типа {staticObjectPurpose}");
        }

        private PropContainerPurpose RollPurpose(IResourceDepositData resourceDepositData)
        {
            return _staticObjectsGeneratorRandomSource.RollPurpose(resourceDepositData);
        }
    }
}